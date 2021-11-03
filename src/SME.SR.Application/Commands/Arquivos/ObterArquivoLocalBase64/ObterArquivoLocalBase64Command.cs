using MediatR;

namespace SME.SR.Application
{
    public class ObterArquivoLocalBase64Command : IRequest<string>
    {
        public ObterArquivoLocalBase64Command(string caminhoArquivo)
        {
            CaminhoArquivo = caminhoArquivo;
        }

        public string CaminhoArquivo { get; }
    }
}
