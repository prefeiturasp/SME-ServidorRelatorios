using System.Collections.Generic;

namespace SME.SR.HtmlPdf
{
    public interface IReportConverter
    {
        byte[] ConvertToPdf(List<string> paginas);
    }
}
