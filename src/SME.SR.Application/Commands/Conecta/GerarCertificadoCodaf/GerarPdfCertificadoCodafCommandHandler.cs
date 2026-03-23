using MediatR;
using SME.SR.HtmlPdf;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application.Commands.Conecta.GerarCertificadoCodaf
{
    public class GerarPdfCertificadoCodafCommandHandler : IRequestHandler<GerarPdfCertificadoCodafCommand, byte[]>
    {
        private readonly IReportConverter _reportConverter;
        public GerarPdfCertificadoCodafCommandHandler(IReportConverter reportConverter)
        {
            _reportConverter = reportConverter ?? throw new ArgumentNullException(nameof(reportConverter));
        }
        public async Task<byte[]> Handle(GerarPdfCertificadoCodafCommand request, CancellationToken cancellationToken)
        {
            var caminhoBase = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "relatorios");
            return _reportConverter.ConvertHtmlToPdfLandscape(request.HtmlCertificado.HtmlContent, caminhoBase, Guid.NewGuid().ToString());
        }

        private static string InserirSequencialNoHtml(string htmlContent, long sequencial)
        {
            var sequencialFormatado = sequencial.ToString("D4");
            var marcador = "{{NUM_SEQ}}";
            if (htmlContent.Contains(marcador))
                htmlContent = htmlContent.Replace(marcador, sequencialFormatado);
            return htmlContent;
        }
    }
}
