using MediatR;
using SME.SR.Infra;

namespace SME.SR.Application
{
    public class TransformarArquivoBase64Command : IRequest<string>
    {
        public ArquivoDto Arquivo { get; set; }

        public TransformarArquivoBase64Command(ArquivoDto arquivo)
        {
            Arquivo = arquivo;
        }
    }
}
