using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class UnificarPdfNAAPACommand : IRequest<bool>
    {
        public UnificarPdfNAAPACommand(string pdfGerado, List<ArquivoDto> anexos, string nomePdfUnificado)
        {
            PdfGerado = pdfGerado;
            Anexos = anexos;
            NomePdfUnificado = nomePdfUnificado;
        }
        
        public string PdfGerado { get; set; }
        public List<ArquivoDto> Anexos { get; set; }
        public string NomePdfUnificado { get; set; }
    }
}
