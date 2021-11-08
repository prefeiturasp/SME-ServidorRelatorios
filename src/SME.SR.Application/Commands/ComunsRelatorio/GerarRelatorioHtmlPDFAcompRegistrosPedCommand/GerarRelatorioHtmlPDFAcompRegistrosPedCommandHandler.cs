using DinkToPdf.Contracts;
using MediatR;
using SME.SR.HtmlPdf;
using SME.SR.Infra;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class GerarRelatorioHtmlPDFAcompRegistrosPedCommandHandler : IRequestHandler<GerarRelatorioHtmlPDFAcompRegistrosPedCommand, string>
    {
        private readonly IServicoFila servicoFila;
        private readonly IHtmlHelper htmlHelper;
        private readonly IReportConverter reportConverter;

        public GerarRelatorioHtmlPDFAcompRegistrosPedCommandHandler(IServicoFila servicoFila,
                                                              IHtmlHelper htmlHelper, IReportConverter reportConverter)
        {
            this.servicoFila = servicoFila ?? throw new ArgumentNullException(nameof(servicoFila));
            this.htmlHelper = htmlHelper ?? throw new ArgumentNullException(nameof(htmlHelper));
            this.reportConverter = reportConverter ?? throw new ArgumentNullException(nameof(reportConverter));
        }

        public Task<string> Handle(GerarRelatorioHtmlPDFAcompRegistrosPedCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
