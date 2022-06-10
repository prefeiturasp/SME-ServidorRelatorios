using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using SME.SR.Infra.Extensions;
using SME.SR.Infra.Utilitarios;
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
            var dto = new ControleGradeDto();

            var modalidadeCalendario = request.Filtros.ModalidadeTurma == Modalidade.EJA ?
                                                ModalidadeTipoCalendario.EJA : request.Filtros.ModalidadeTurma == Modalidade.Infantil ?
                                                    ModalidadeTipoCalendario.Infantil : ModalidadeTipoCalendario.FundamentalMedio;
            var tipoCalendarioId = await mediator.Send(new ObterIdTipoCalendarioPorAnoLetivoEModalidadeQuery(request.Filtros.AnoLetivo, modalidadeCalendario, request.Filtros.Semestre));
            foreach (long turmaId in request.Filtros.Turmas)
            {
                var aulasPrevistasTurma = new List<AulaPrevistaBimestreQuantidade>();
                foreach (long componenteCurricularId in request.Filtros.ComponentesCurriculares)
                {
                    var aulasPrevistasBimestresComponente = await mediator.Send(new ObterAulasPrevistasDadasQuery(turmaId, componenteCurricularId, tipoCalendarioId));

                    if (aulasPrevistasBimestresComponente.Any())
                        aulasPrevistasTurma.AddRange(aulasPrevistasBimestresComponente);
                }

                if (aulasPrevistasTurma.Any())
                    dto.Turmas.Add(await MapearParaTurmaDto(aulasPrevistasTurma, request.Filtros.Bimestres, turmaId, tipoCalendarioId, request.Filtros.ModalidadeTurma));
            }

            await MontarCabecalhoRelatorioDto(dto, request.Filtros);

            return !string.IsNullOrEmpty(await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("RelatorioControleGradeSintetico", dto, request.CodigoCorrelacao, "", "Relatório Controle de Grade Sintético", true, "RELATÓRIO CONTROLE DE GRADE SINTÉTICO")));
        }

        private async Task <IEnumerable<Turma>> ObterTurmas(IEnumerable<long> turmasIds)
        {
            var turmas = await mediator.Send(new ObterTurmasPorIdsQuery(turmasIds.ToArray()));
            return turmas;
        }

        private async Task<IEnumerable<ComponenteCurricularPorTurma>> ObterComponentesCurriculares(IEnumerable<long> componentesCurriculares)
        {
            var componentes = await mediator.Send(new ObterComponentesCurricularesEolPorIdsQuery(componentesCurriculares.ToArray()));
            return componentes;
        }

        private async Task<IEnumerable<PeriodoEscolar>> ObterPeriodosEscolares(IEnumerable<int> bimestres, long tipoCalendarioId)
        {
            var periodos = await mediator.Send(new ObterPeriodosEscolaresPorTipoCalendarioQuery(tipoCalendarioId));
            return periodos.Where(a => bimestres.Contains(a.Bimestre));
        }

        private async Task<TurmaControleGradeDto> MapearParaTurmaDto(List<AulaPrevistaBimestreQuantidade> aulasPrevistasTurma, IEnumerable<int> bimestres, long turmaId, long tipoCalendarioId, Modalidade modalidadeTurma)
        {
            var turmaDto = new TurmaControleGradeDto()
            {
                Nome = aulasPrevistasTurma.FirstOrDefault().TurmaNome
            };

            foreach (var bimestre in bimestres.OrderBy(o => o))
            {
                turmaDto.Bimestres.Add(await MapearParaBimestreDto(bimestre, aulasPrevistasTurma, turmaId, tipoCalendarioId, modalidadeTurma));
            }

            return turmaDto;
        }

        private async Task<BimestreControleGradeDto> MapearParaBimestreDto(int bimestre, List<AulaPrevistaBimestreQuantidade> aulasPrevistasTurma, long turmaId, long tipoCalendarioId, Modalidade modalidadeTurma)
        {
            var aulasPrevistasBimestre = aulasPrevistasTurma.Where(c => c.Bimestre == bimestre);

            var dataInicio = aulasPrevistasBimestre.FirstOrDefault().DataInicio;
            var dataFim = aulasPrevistasBimestre.FirstOrDefault().DataFim;

            var bimestreDto = new BimestreControleGradeDto()
            {
                Descricao = $"{bimestre}º Bimestre - {dataInicio.ToString("dd/MM")} À {dataFim.ToString("dd/MM")}"
            };

            foreach (var aulasPrevistasComponente in aulasPrevistasBimestre.OrderBy(c => c.ComponenteCurricularNome))
            {
                bimestreDto.ComponentesCurriculares.Add(await MapearParaComponenteDto(aulasPrevistasComponente, turmaId, tipoCalendarioId, modalidadeTurma));
            }

            return bimestreDto;
        }

        private async Task<ComponenteCurricularControleGradeDto> MapearParaComponenteDto(AulaPrevistaBimestreQuantidade aulasPrevistasComponente, long turmaId, long tipoCalendarioId, Modalidade modalidadeTurma)
        {
            var componenteDto = new ComponenteCurricularControleGradeDto();

            componenteDto.Nome = aulasPrevistasComponente.ComponenteCurricularNome;
            componenteDto.AulasPrevistas = aulasPrevistasComponente.Previstas;
            componenteDto.AulasCriadasProfessorTitular = aulasPrevistasComponente.CriadasTitular;
            componenteDto.AulasCriadasProfessorSubstituto = aulasPrevistasComponente.CriadasCJ;
            componenteDto.AulasDadasProfessorTitular = aulasPrevistasComponente.CumpridasTitular;
            componenteDto.AulasDadasProfessorSubstituto = aulasPrevistasComponente.CumpridasCj;
            componenteDto.Repostas = aulasPrevistasComponente.Reposicoes;
            componenteDto.Divergencias = await VerificarDivergencias(turmaId, aulasPrevistasComponente.Bimestre, aulasPrevistasComponente.ComponenteCurricularId, aulasPrevistasComponente.Regencia, modalidadeTurma,
                componenteDto.AulasPrevistas != (aulasPrevistasComponente.CumpridasTitular + aulasPrevistasComponente.CumpridasCj), tipoCalendarioId, componenteDto.AulasPrevistas == 0) ? "Sim" : "Não";

            return componenteDto;
        }

        private async Task<bool> VerificarDivergencias(long turmaId, int bimestre, long componenteCurricularId, bool regencia, Modalidade modalidadeTurma, bool divergenciaNumeroAulas, long tipoCalendarioId, bool aulasPrevistasvZero)
        {
            //Não existe aulas previstas cadastradas.
            if (aulasPrevistasvZero)
                return true;

            //Diferença entre aulas previstas X aulas dadas.
            if (divergenciaNumeroAulas)
                return true;

            //Não há nenhuma aula criada para o componente curricular.
            var verificaExisteAula = await mediator.Send(new VerificaExisteAulaPorTurmaCodigoEComponenteCurricularIdQuery(turmaId, componenteCurricularId.ToString(),bimestre, tipoCalendarioId));
            if (!verificaExisteAula)
                return true;

            //2 ou mais aulas normais criadas no mesmo dia por um professor de regência de classe do fundamental na mesma turma
            if (regencia)
            {
                var verificaAulaCriadaProfessor = await mediator.Send(new VerificarAulasNormaisCriadasProfessorRegenciaQuery(componenteCurricularId.ToString(), bimestre, tipoCalendarioId));
                if (!verificaAulaCriadaProfessor)
                    return true;
            }            

            //Mais de um registro de aula normal do mesmo professor, componente curricular e turma no mesmo dia.           
            var verificaExisteMaisAula = await mediator.Send(new VerificaExisteMaisAulaCadastradaNoDiaQuery(turmaId, componenteCurricularId.ToString(), tipoCalendarioId, bimestre));
            if (verificaExisteMaisAula)
                return true;

            if (await mediator.Send(new VerificaExisteAulaTitularECjQuery(turmaId, componenteCurricularId, tipoCalendarioId, bimestre)))
                return true;

            if (await VerificaAulasMesmoDiaEProfessor(turmaId, componenteCurricularId, tipoCalendarioId, bimestre, regencia, modalidadeTurma))
                return true;

            return false;
        }

        private async Task<bool> VerificaAulasMesmoDiaEProfessor(long turmaId, long componenteCurricularId, long tipoCalendarioId, int bimestre, bool regencia, Modalidade modalidadeTurma)
        {
            if (!regencia && !(modalidadeTurma == Modalidade.EJA) &&
                await mediator.Send(new VerificaExisteAulasMemoDiaEProfessorQuery(turmaId, componenteCurricularId, tipoCalendarioId, bimestre)))
                return true;

            return false;
        }

        private async Task MontarCabecalhoRelatorioDto(ControleGradeDto dto, RelatorioControleGradeFiltroDto filtros)
        {
            var turmaId = filtros.Turmas.First();
            var turma = await mediator.Send(new ObterTurmaResumoComDreUePorIdQuery(turmaId));
                
            dto.Filtro.Dre = turma.Ue.Dre.Abreviacao;
            dto.Filtro.Ue = $"{turma.Ue.CodigoUe} - {turma.Ue.TipoEscola.ShortName()} {turma.Ue.Nome}";
            dto.Filtro.Turma = filtros.Turmas.Count() > 1 ? "Todas" : $"{turma.Modalidade.ShortName()} - {turma.Nome}";
            dto.Filtro.Bimestre = filtros.Bimestres.Count() == QuantidadePeriodosPorModalidade(turma.Modalidade) ?
                                    "Todos" : string.Join(",", filtros.Bimestres);
            dto.Filtro.ComponenteCurricular = ObterNomeComponente(filtros, dto);
            dto.Filtro.Usuario = filtros.UsuarioNome;
            dto.Filtro.RF = filtros.UsuarioRf;
            dto.Filtro.EhEducacaoInfantil = filtros.ModalidadeTurma == Modalidade.Infantil;
        }

        private int QuantidadePeriodosPorModalidade(Modalidade modalidade)
            => modalidade == Modalidade.EJA ? 2 : 4;

        private string ObterNomeComponente(RelatorioControleGradeFiltroDto filtros, ControleGradeDto dto)
        {
            return filtros.ComponentesCurriculares.Count() > 1 ?
                            "Todos" : !dto.Turmas.Any()
                                   || !dto.Turmas.First().Bimestres.Any()
                                   || !dto.Turmas.First().Bimestres.First().ComponentesCurriculares.Any() ?
                                "" : dto.Turmas.First().Bimestres.First().ComponentesCurriculares.First().Nome;
        }
    }
}
