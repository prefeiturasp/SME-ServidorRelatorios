using System.Text.RegularExpressions;

namespace SME.SR.Infra.Utilitarios
{
    public static class UtilHtml
    {
        public static string FormatarHtmlParaTexto(string textoHtml)
        {
            if (!string.IsNullOrEmpty(textoHtml))
            {
                string semTags = UtilRegex.RemoverTagsHtmlMidia(textoHtml);
                semTags = RemoverTagsStyle(semTags);
                semTags = UtilRegex.RemoverTagsHtml(semTags);
                semTags = UtilRegex.AdicionarEspacos(semTags);
                
                return semTags;
            }
            else
            {
                return textoHtml;
            }
        }

        private static string RemoverTagsStyle(string textoHtml)
        {
            var posicao = textoHtml.IndexOf("<style>");
            if (posicao > 0)
            {
                var posicaoFinal = textoHtml.IndexOf("</style>");
                if (posicaoFinal > 0)
                    return textoHtml.Substring(0, posicao) + textoHtml.Substring(posicaoFinal + 8);
            }

            return textoHtml;
        }
    }
}
