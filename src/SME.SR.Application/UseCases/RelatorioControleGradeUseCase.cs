using MediatR;
using SME.SR.Application.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Text;
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

                await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("RelatorioControleGrade", null, request.CodigoCorrelacao));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
