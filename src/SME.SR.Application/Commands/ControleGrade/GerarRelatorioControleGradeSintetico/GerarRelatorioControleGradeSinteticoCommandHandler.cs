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
            foreach (long turmaId in request.Filtros.Turmas)
            {
                var aulasPrevistasTurma = new List<AulaPrevistaBimestreQuantidade>();
                foreach (long componenteCurricularId in request.Filtros.ComponentesCurriculares)
                {
                    var aulasPrevistasBimestresComponente = await mediator.Send(new ObterAulasPrevistasDadasQuery(turmaId, componenteCurricularId));

                    if (aulasPrevistasBimestresComponente != null)
                        aulasPrevistasTurma.AddRange(aulasPrevistasBimestresComponente);
                }

                dto.Turmas.Add(MapearParaTurmaDto(aulasPrevistasTurma, request.Filtros.Bimestres));
            }

            await MontarCabecalhoRelatorioDto(dto, request.Filtros);

            return await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("RelatorioControleGradeSintetico", dto, request.CodigoCorrelacao));
        }

        private TurmaControleGradeSinteticoDto MapearParaTurmaDto(List<AulaPrevistaBimestreQuantidade> aulasPrevistasTurma, IEnumerable<int> bimestres)
        {
            var turmaDto = new TurmaControleGradeSinteticoDto()
            {
                Nome = aulasPrevistasTurma.FirstOrDefault().TurmaNome
            };

            foreach (var bimestre in bimestres.OrderBy(o => o))
            {
                turmaDto.Bimestres.Add(MapearParaBimestreDto(bimestre, aulasPrevistasTurma));
            }

            return turmaDto;
        }

        private BimestreControleGradeSinteticoDto MapearParaBimestreDto(int bimestre, List<AulaPrevistaBimestreQuantidade> aulasPrevistasTurma)
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
                bimestreDto.ComponentesCurriculares.Add(MapearParaComponenteDto(aulasPrevistasComponente));
            }

            return bimestreDto;
        }

        private ComponenteCurricularControleGradeSinteticoDto MapearParaComponenteDto(AulaPrevistaBimestreQuantidade aulasPrevistasComponente)
        {
            var componenteDto = new ComponenteCurricularControleGradeSinteticoDto();

            componenteDto.Nome = aulasPrevistasComponente.ComponenteCurricularNome;
            componenteDto.AulasPrevistas = aulasPrevistasComponente.Previstas;
            componenteDto.AulasCriadasProfessorTitular = aulasPrevistasComponente.CriadasTitular;
            componenteDto.AulasCriadasProfessorSubstituto = aulasPrevistasComponente.CriadasCJ;
            componenteDto.AulasDadasProfessorTitular = aulasPrevistasComponente.CumpridasTitular;
            componenteDto.AulasDadasProfessorSubstituto = aulasPrevistasComponente.CumpridasCj;
            componenteDto.Repostas = aulasPrevistasComponente.Reposicoes;

            return componenteDto;
        }

        private async Task MontarCabecalhoRelatorioDto(ControleGradeSinteticoDto dto, RelatorioControleGradeFiltroDto filtros)
        {
            var turmaId = filtros.Turmas.First();
            var turma = await mediator.Send(new ObterTurmaResumoComDreUePorIdQuery(turmaId));

            dto.Filtro.Dre = turma.Ue.Dre.Abreviacao;
            dto.Filtro.Ue = turma.Ue.Nome;
            dto.Filtro.Turma = filtros.Turmas.Count() > 1 ? "Todas" : turma.Nome ;
            dto.Filtro.Bimestre = filtros.Bimestres.Count() == (turma.Modalidade == Modalidade.EJA ? 2 : 4) ?
                                    "Todos" : string.Join(",", filtros.Bimestres);
            dto.Filtro.ComponenteCurricular = filtros.ComponentesCurriculares.Count() > 1 ? 
                            "Todos" : dto.Turmas.First().Bimestres.First().ComponentesCurriculares.First().Nome;
            dto.Filtro.Usuario = filtros.UsuarioNome;
            dto.Filtro.RF = filtros.UsuarioRf;
        }
    }
}
