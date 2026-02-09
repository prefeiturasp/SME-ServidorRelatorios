using HtmlAgilityPack;

namespace SME.SR.Infra.Extensions
{
    public static class StringExtension
    {
        public static string LimparFormatacaoHtml(this string str)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(str);            
            str = htmlDoc.DocumentNode.InnerText;
            str = str.Replace("&gt;", "");
            return str;
        }

        public static string FormatarPrimeiraMaiuscula(this string input)
        {
            if (input.Length < 1)
                return input;

            var sentence = input.ToLower();
            return sentence[0].ToString().ToUpper() +
               sentence.Substring(1);
        }
        
        public static bool EstaFiltrandoTodas(this string filtro)
        {
            return filtro is "-99" || string.IsNullOrEmpty(filtro);
        }

        public static int ConverterHoraMinutoParaInteiro(this string? horasTexto)
        {
            if (string.IsNullOrWhiteSpace(horasTexto)) return 0;

            var partes = horasTexto.Split(':');
            if (partes.Length != 2) return 0;

            if (int.TryParse(partes[0], out int horas) && int.TryParse(partes[1], out int minutos))
            {
                return (horas * 60 + minutos) / 60;
            }
            return 0;
        }
    }
}
