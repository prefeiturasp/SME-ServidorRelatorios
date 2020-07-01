using RazorEngine;
using RazorEngine.Templating;

namespace SME.SR.HtmlPdf
{
    public class RazorProcessor
    {
        public string ProcessarTemplate<T>(T model, string template, string nomeTemplate)
        {
            var result = Engine.Razor.RunCompile(template, nomeTemplate, typeof(T), model);

            return result;
        }
    }
}
