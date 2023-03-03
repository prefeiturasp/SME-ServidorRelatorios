using MediatR;
using SME.SR.Application.Interfaces;
using SME.SR.Data;
using SME.SR.Infra;
using SME.SR.Infra.Utilitarios;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class RelatorioEncaminhamentosNAAPAUseCase : IRelatorioEncaminhamentosNAAPAUseCase
    {
        private readonly IMediator mediator;

        public RelatorioEncaminhamentosNAAPAUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Executar(FiltroRelatorioDto request)
        {
            var filtroRelatorio = request.ObterObjetoFiltro<FiltroRelatorioEncaminhamentoNAAPADto>();
            var encaminhamentosNAAPA = await mediator.Send(new ObterEncaminhamentosNAAPAQuery(filtroRelatorio));

            if (encaminhamentosNAAPA == null || !encaminhamentosNAAPA.Any())
                throw new NegocioException("Nenhuma informação para os filtros informados.");

            
            //await mediator.Send(new GerarRelatorioHtmlPDFEncaminhamentosAeeCommand(cabecalho, encaminhamentosAgrupados, request.CodigoCorrelacao));
        }
    }
}
