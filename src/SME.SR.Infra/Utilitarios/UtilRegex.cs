using System;
using System.Text.RegularExpressions;
using System.Timers;

namespace SME.SR.Infra.Utilitarios
{
    public static class UtilRegex
    {
        public static string RemoverTagsHtml(string texto)
        {
            texto = Regex.Replace(texto, @"<[^>]*>", String.Empty);
            texto = Regex.Replace(texto, @"&nbsp;", " ").Trim();
            return texto;
        }

        public static string RemoverTagsHtmlMidia(string texto)
        {
            texto = Regex.Replace(texto, @"<img[^>]*>", " [arquivo indisponível na impressão] ");
            texto = Regex.Replace(texto, @"<iframe[^>]*>", " [arquivo indisponível na impressão] ");
            return texto;
        }
    }
}
