using MediatR;

namespace SME.SR.Application
{
    public class ObterHtmlComImagensBase64Query : IRequest<string>
    {
        public ObterHtmlComImagensBase64Query(string html)
        {
            Html = html;
        }

        public string Html { get; }
    }
}
