using RazorEngine;
using RazorEngine.Templating;
using Microsoft.AspNetCore.Razor.Language;

namespace SME.SR.HtmlPdf
{
    public class RazorProcessor
    {
        public string ProcessarTemplate<T>(T model, string template, string nomeTemplate)
        {
            var result = Engine.Razor.RunCompile(template, nomeTemplate, null, model);

            return result;
        }
    }
}
