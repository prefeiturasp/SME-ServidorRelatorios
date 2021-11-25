namespace SME.SR.Infra.Utilitarios
{
    public static class UtilHtml
    {
        public static string FormatarHtmlParaTexto(string textoHtml)
        {
            if (!string.IsNullOrEmpty(textoHtml))
            {
                string semTags = UtilRegex.RemoverTagsHtmlMidia(textoHtml);
                semTags = UtilRegex.RemoverTagsHtml(semTags);
                semTags = UtilRegex.AdicionarEspacos(semTags);

                return semTags;
            }
            else
            {
                return textoHtml;
            }
        }
    }
}
