using HtmlAgilityPack;

namespace SME.SR.Infra.Extensions
{
    public static class StringExtension
    {
        public static string LimparFormatacaoHtml(this string str)
        {
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(str);            
            str = htmlDoc.DocumentNode.InnerText;
            str = str.Replace("&gt;", "");
            return str;
        }
    }
}
