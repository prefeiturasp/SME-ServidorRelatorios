using MediatR;

namespace SME.SR.Application
{
    public class ObterArquivoRemotoBase64Command : IRequest<string>
    {
        public ObterArquivoRemotoBase64Command(string url)
        {
            Url = url;
        }

        public string Url { get; }
    }
}
