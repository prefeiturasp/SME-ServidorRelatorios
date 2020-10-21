using MediatR;
using SME.SR.Application.Interfaces;
using SME.SR.Infra;
using System;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class RelatorioControleGradeUseCase : IRelatorioControleGradeUseCase
    {
        private readonly IMediator mediator;

        public RelatorioControleGradeUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Executar(FiltroRelatorioDto request)
        {
            try
            {
                var filtros = request.ObterObjetoFiltro<RelatorioControleGradeFiltroDto>();

                // TODO carregar dados do relatório sintético / analítico

                foreach (long turmaId in filtros.Turmas)
                {
                    foreach (int bimestre in filtros.Bimestres)
                    {
                        foreach (long componenteCurricularId in filtros.ComponentesCurriculares)
                        {
                            var turma = await mediator.Send(new ObterTurmaQuery(turmaId.ToString()));

                            var tipoCalendarioId = await mediator.Send(new ObterIdTipoCalendarioPorAnoLetivoEModalidadeQuery(turma.AnoLetivo, turma.ModalidadeTipoCalendario, bimestre));

                            var aulaPrevista = await mediator.Send(new ObterAulasPrevistasDadasQuery(tipoCalendarioId, turmaId.ToString(), componenteCurricularId.ToString()));

                            //IEnumerable<AulaPrevistaBimestreQuantidade> aulaPrevistaBimestres;

                            // aulaPrevistaBimestres = await mediator.Send(new ObterAulasDadasNoBimestreQuery(turmaId.ToString(), tipoCalendarioId, componenteCurricularId, bimestre));


                        }
                    }
                }

                await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("RelatorioControleGrade", null, request.CodigoCorrelacao));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
