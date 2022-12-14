using System.Runtime.CompilerServices;
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

        public static string FormatarPrimeiraMaiuscula(this string input)
        {
            if (input.Length < 1)
                return input;

            string sentence = input.ToLower();
            return sentence[0].ToString().ToUpper() +
               sentence.Substring(1);
        }
        
        public static bool EstaFiltrandoTodas(this string filtro)
        {
            return filtro.Equals("-99");
        }
    }
}
