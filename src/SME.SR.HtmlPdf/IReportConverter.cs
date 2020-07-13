using System.Collections.Generic;

namespace SME.SR.HtmlPdf
{
    public interface IReportConverter
    {
        byte[] ConvertToPdf(List<string> paginas);
        void Converter(string html, string nomeArquivo);
    }
}
