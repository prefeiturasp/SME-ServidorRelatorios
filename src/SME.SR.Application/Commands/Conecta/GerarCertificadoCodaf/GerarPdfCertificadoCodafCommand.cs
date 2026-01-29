using MediatR;
using SME.SR.Infra.Dtos.Relatorios.Conecta;

namespace SME.SR.Application.Commands.Conecta.GerarCertificadoCodaf
{
    public class GerarPdfCertificadoCodafCommand : IRequest<byte[]>
    {
        public HtmlCertificadoCodafDto HtmlCertificado { get; set; }
        public GerarPdfCertificadoCodafCommand(HtmlCertificadoCodafDto htmlCertificado)
        {
            HtmlCertificado = htmlCertificado;
        }
    }
}
