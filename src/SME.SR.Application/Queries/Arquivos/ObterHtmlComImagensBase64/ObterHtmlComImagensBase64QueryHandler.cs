using HtmlAgilityPack;
using MediatR;
using SME.SR.Infra.Utilitarios;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterHtmlComImagensBase64QueryHandler : IRequestHandler<ObterHtmlComImagensBase64Query, string>
    {
        private readonly IMediator mediator;
        private const string REFERENCIA_IMAGEM_URL_SGP = "novosgp";
        private const string PROTOCOLO_HTTP = "http:";
        private const string PROTOCOLO_HTTPS = "https:";
        private const string INICIAL_ORIGEM_IMAGEM_BLOB = "blob:";
        private const string REFERENCIA_NOME_PASTA_TEMPORARIA = "temp";
        private const string REFERENCIA_NOME_PASTA_ARQUIVOS = "/ARQUIVOS/";

        public ObterHtmlComImagensBase64QueryHandler(IMediator mediator)
        {
            this.mediator = mediator ?? throw new System.ArgumentNullException(nameof(mediator));
        }

        public async Task<string> Handle(ObterHtmlComImagensBase64Query request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.Html))
                return string.Empty;

            var registroFormatado = UtilRegex.RemoverTagsHtmlVideo(request.Html);

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(registroFormatado);
            var nodes = doc.DocumentNode.SelectNodes(@"//img[@src]");
            if (nodes != null)
                foreach (var img in nodes)
                {
                    var caminho = img.Attributes["src"].Value;
                    var arquivoBase64 = FormatoAceitoParaImpressaoImagem(caminho) ? await ObterArquivoRemotoBase64(caminho, request.EscalaHorizontal, request.EscalaVertical) : string.Empty;

                    registroFormatado = registroFormatado.Replace(caminho, arquivoBase64);
                }

            return registroFormatado;
        }

        private async Task<string> ObterArquivo(string caminho, float escalaHorizontal, float escalaVertical)
            => await ObterArquivoRemotoBase64(caminho, escalaHorizontal, escalaVertical) ?? await ObterArquivoLocalBase64(caminho);

        private async Task<string> ObterArquivoRemotoBase64(string url, float escalaHorizontal, float escalaVertical)
        {
            return await mediator.Send(new ObterArquivoRemotoBase64Command(url, escalaHorizontal, escalaVertical));
        }

        private async Task<string> ObterArquivoLocalBase64(string url)
        {
            var posicao = url.IndexOf("/Arquivos/");
            var caminho = url.Substring(posicao, url.Length - posicao);
            return await mediator.Send(new ObterArquivoLocalBase64Command(caminho));
        }
        private bool FormatoAceitoParaImpressaoImagem(string url)
        {
            if (url.Contains(PROTOCOLO_HTTP) || url.Contains(PROTOCOLO_HTTPS) || url.StartsWith(INICIAL_ORIGEM_IMAGEM_BLOB))
            {
                if (!url.Contains(REFERENCIA_IMAGEM_URL_SGP) || (url.Contains(REFERENCIA_IMAGEM_URL_SGP) && url.Contains(REFERENCIA_NOME_PASTA_TEMPORARIA)))
                    return false;
                else if (url.Contains(REFERENCIA_IMAGEM_URL_SGP))
                    return url.ToUpper().Contains(REFERENCIA_NOME_PASTA_ARQUIVOS);
            }

            return true;
        }
    }
}
