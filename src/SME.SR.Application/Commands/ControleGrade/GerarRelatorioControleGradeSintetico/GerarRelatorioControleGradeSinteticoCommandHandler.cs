using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class GerarRelatorioControleGradeSinteticoCommandHandler : IRequestHandler<GerarRelatorioControleGradeSinteticoCommand, bool>
    {
        private readonly IMediator mediator;

        public GerarRelatorioControleGradeSinteticoCommandHandler(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<bool> Handle(GerarRelatorioControleGradeSinteticoCommand request, CancellationToken cancellationToken)
        {
            var dto = new ControleGradeSinteticoDto();

            var modalidadeCalendario = request.Filtros.ModalidadeTurma == Modalidade.EJA ?
                                                ModalidadeTipoCalendario.EJA : request.Filtros.ModalidadeTurma == Modalidade.Infantil ?
                                                    ModalidadeTipoCalendario.Infantil : ModalidadeTipoCalendario.FundamentalMedio;
            var tipoCalendarioId = await mediator.Send(new ObterIdTipoCalendarioPorAnoLetivoEModalidadeQuery(request.Filtros.AnoLetivo, modalidadeCalendario, request.Filtros.Semestre));

            foreach (long turmaId in request.Filtros.Turmas)
            {
                var aulasPrevistasTurma = new List<AulaPrevistaBimestreQuantidade>();
                foreach (long componenteCurricularId in request.Filtros.ComponentesCurriculares)
                {
                    var aulasPrevistasBimestresComponente = await mediator.Send(new ObterAulasPrevistasDadasQuery(turmaId, componenteCurricularId));

                    if (aulasPrevistasBimestresComponente != null)
                    {
                        aulasPrevistasTurma.AddRange(aulasPrevistasBimestresComponente);
                    }
                }

                dto.Turmas.Add(await MapearParaTurmaDto(aulasPrevistasTurma, request.Filtros.Bimestres, turmaId, tipoCalendarioId));
            }

            MontarCabecalhoRelatorioDto(dto, request.Filtros);

            return await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("RelatorioControleGradeSintetico", dto, request.CodigoCorrelacao));
        }

        private async Task<TurmaControleGradeSinteticoDto> MapearParaTurmaDto(List<AulaPrevistaBimestreQuantidade> aulasPrevistasTurma, IEnumerable<int> bimestres, long turmaId, long tipoCalendarioId)
        {
            var turmaDto = new TurmaControleGradeSinteticoDto()
            {
                Nome = aulasPrevistasTurma.FirstOrDefault().TurmaNome
            };

            foreach (var bimestre in bimestres.OrderBy(o => o))
            {
                turmaDto.Bimestres.Add(await MapearParaBimestreDto(bimestre, aulasPrevistasTurma, turmaId, tipoCalendarioId));
            }

            return turmaDto;
        }

        private async Task<BimestreControleGradeSinteticoDto> MapearParaBimestreDto(int bimestre, List<AulaPrevistaBimestreQuantidade> aulasPrevistasTurma, long turmaId, long tipoCalendarioId)
        {
            var aulasPrevistasBimestre = aulasPrevistasTurma.Where(c => c.Bimestre == bimestre);

            var dataInicio = aulasPrevistasBimestre.FirstOrDefault().DataInicio;
            var dataFim = aulasPrevistasBimestre.FirstOrDefault().DataFim;

            var bimestreDto = new BimestreControleGradeSinteticoDto()
            {
                Descricao = $"{bimestre}º Bimestre - {dataInicio.ToString("dd/MM")} À {dataFim.ToString("dd/MM")}"
            };

            foreach (var aulasPrevistasComponente in aulasPrevistasBimestre.OrderBy(c => c.ComponenteCurricularNome))
            {
                bimestreDto.ComponentesCurriculares.Add(await MapearParaComponenteDto(aulasPrevistasComponente, turmaId, tipoCalendarioId));
            }

            return bimestreDto;
        }

        private async Task<ComponenteCurricularControleGradeSinteticoDto> MapearParaComponenteDto(AulaPrevistaBimestreQuantidade aulasPrevistasComponente, long turmaId, long tipoCalendarioId)
        {
            var componenteDto = new ComponenteCurricularControleGradeSinteticoDto();

            componenteDto.Nome = aulasPrevistasComponente.ComponenteCurricularNome;
            componenteDto.AulasPrevistas = aulasPrevistasComponente.Previstas;
            componenteDto.AulasCriadasProfessorTitular = aulasPrevistasComponente.CriadasTitular;
            componenteDto.AulasCriadasProfessorSubstituto = aulasPrevistasComponente.CriadasCJ;
            componenteDto.AulasDadasProfessorTitular = aulasPrevistasComponente.CumpridasTitular;
            componenteDto.AulasDadasProfessorSubstituto = aulasPrevistasComponente.CumpridasCj;
            componenteDto.Repostas = aulasPrevistasComponente.Reposicoes;
            componenteDto.Divergencias = await VerificarDivergencias(turmaId, aulasPrevistasComponente.Bimestre, aulasPrevistasComponente.ComponenteCurricularId, 
                componenteDto.AulasPrevistas != (aulasPrevistasComponente.CumpridasTitular + aulasPrevistasComponente.CumpridasCj), tipoCalendarioId) ? "Sim" : "Não";

            return componenteDto;
        }

        private async Task<bool> VerificarDivergencias(long turmaId, int bimestre, long componenteCurricularId, bool divergenciaNumeroAulas, long tipoCalendarioId)
        {
            //Diferença entre aulas previstas X aulas dadas.
            //if (divergenciaNumeroAulas)
            //    return true;

            //Não há nenhuma aula criada para o componente curricular.
            var verificaExisteAula = await mediator.Send(new VerificaExisteAulaPorTurmaCodigoEComponenteCurricularIdQuery(turmaId, componenteCurricularId.ToString(),bimestre, tipoCalendarioId));
            if (!verificaExisteAula)
                return true;

            //2 ou mais aulas normais criadas no mesmo dia por um professor de regência de classe do fundamental na mesma turma
            var verificaAulaCriadaProfessor = await mediator.Send(new VerificarAulasNormaisCriadasProfessorRegenciaQuery(turmaId, componenteCurricularId.ToString()));
            if (!verificaAulaCriadaProfessor)
                return true;

            //Mais de um registro de aula normal do mesmo professor, componente curricular e turma no mesmo dia.
            var verificaExisteMaisAula = await mediator.Send(new VerificaExisteMaisAulaCadastradaNoDiaQuery(turmaId, componenteCurricularId.ToString(), tipoCalendarioId, bimestre));
            if (verificaExisteMaisAula)
                return true;

            if (await mediator.Send(new VerificaExisteAulaTitularECjQuery(turmaId, componenteCurricularId, tipoCalendarioId, bimestre)))
                return true;

            return false;
        }

        private async Task MontarCabecalhoRelatorioDto(ControleGradeSinteticoDto dto, RelatorioControleGradeFiltroDto filtros)
        {
            var turmaId = filtros.Turmas.First();
            var turma = await mediator.Send(new ObterTurmaResumoComDreUePorIdQuery(turmaId));
                
            dto.Filtro.Dre = turma.Ue.Dre.Abreviacao;
            dto.Filtro.Ue = turma.Ue.Nome;
            dto.Filtro.Turma = filtros.Turmas.Count() > 1 ? "Todas" : turma.Nome;
            dto.Filtro.Bimestre = filtros.Bimestres.Count() == (turma.Modalidade == Modalidade.EJA ? 2 : 4) ?
                                    "Todos" : string.Join(",", filtros.Bimestres);
            dto.Filtro.ComponenteCurricular = filtros.ComponentesCurriculares.Count() > 1 ?
                            "Todos" : dto.Turmas.First().Bimestres.First().ComponentesCurriculares.First().Nome;
            dto.Filtro.Usuario = filtros.UsuarioNome;
            dto.Filtro.RF = filtros.UsuarioRf;
        }
    }
}
