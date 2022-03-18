using MediatR;

namespace SME.SR.Application
{
    public class ObterArquivoRemotoBase64Command : IRequest<string>
    {
        public ObterArquivoRemotoBase64Command(string url, float escalaHorizontal, float escalaVertical)
        {
            Url = url;
            EscalaHorizontal = escalaHorizontal;
            EscalaVertical = escalaVertical;
        }

        public string Url { get; }
        public float EscalaHorizontal { get; set; }
        public float EscalaVertical { get; set; }
    }
}
