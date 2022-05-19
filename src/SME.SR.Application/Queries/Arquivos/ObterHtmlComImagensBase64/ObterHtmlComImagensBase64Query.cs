using MediatR;

namespace SME.SR.Application
{
    public class ObterHtmlComImagensBase64Query : IRequest<string>
    {
        public ObterHtmlComImagensBase64Query(string html, float escalaHorizontal = 750f, float escalaVertical = 800f)
        {
            Html = html;
            EscalaHorizontal = escalaHorizontal;
            EscalaVertical = escalaVertical;
        }

        public string Html { get; }
        public float EscalaHorizontal { get; set; }
        public float EscalaVertical { get; set; }
    }
}
