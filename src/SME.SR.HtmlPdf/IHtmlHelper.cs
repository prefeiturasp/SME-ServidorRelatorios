using Microsoft.AspNetCore.Mvc.ViewEngines;
using System.Threading.Tasks;

namespace SME.SR.HtmlPdf
{
    public interface IHtmlHelper
    {
        Task<string> RenderRazorViewToString(string viewName, object model);
    }
}