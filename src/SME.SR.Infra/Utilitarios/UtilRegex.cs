using System;
using System.Text.RegularExpressions;

namespace SME.SR.Infra.Utilitarios
{
    public static class UtilRegex
    {
        public static string RemoverTagsHtml(string texto)
        {
            if (string.IsNullOrEmpty(texto))
                return texto?.Trim();

            texto = Regex.Replace(texto, @"<style[^>]*>.*?</style>", "", RegexOptions.Singleline);
            texto = Regex.Replace(texto, @"<span[^>]*>", " ");
            texto = Regex.Replace(texto, @"<div[^>]*>", " ");
            texto = Regex.Replace(texto, @"<br[^>]*>", " ");
            texto = Regex.Replace(texto, @"<p[^>]*>", " ");
            texto = Regex.Replace(texto, @"<li[^>]*>", " ");
            texto = Regex.Replace(texto, @"<[^>]*>", String.Empty);
            texto = Regex.Replace(texto, @"&nbsp;", " ").Trim();
            return texto.Trim();
        }

        public static string RemoverTagsLink(string texto)
        {
            texto = Regex.Replace(texto, @"<a[^>]*>", " ");
            return texto.Trim();
        }

        public static string RemoverTagsHtmlMidia(string texto)
        {
            texto = Regex.Replace(texto, @"<img[^>]*>", " [arquivo indisponível na impressão] ");
            texto = Regex.Replace(texto, @"<iframe[^>]*>", " [arquivo indisponível na impressão] ");
            return texto;
        }
        public static string RemoverTagsHtmlMultiMidia(string texto)
        {
            texto = Regex.Replace(texto, @"<img[^>]*>", " [arquivo indisponível na impressão] ");
            texto = Regex.Replace(texto, @"<iframe[^>]*>", " [arquivo indisponível na impressão] ");
            texto = Regex.Replace(texto, @"<video.+</video>", " [arquivo indisponível na impressão] ");
            return texto;
        }

        public static string RemoverTagsHtmlVideo(string texto)
        {
            texto = Regex.Replace(texto, @"<iframe[^>]*>", " [arquivo indisponível na impressão] ");
            texto = Regex.Replace(texto, @"<video.+</video>", " [arquivo indisponível na impressão] ");
            return texto;
        }

        public static string AdicionarEspacos(string texto)
        {
            texto = Regex.Replace(texto, @"\.(?! |$)", ". ");
            return texto;
        }
    }
}
