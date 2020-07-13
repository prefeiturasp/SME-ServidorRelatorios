using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SME.SR.HtmlPdf
{

    public class HtmlHelper : IHtmlHelper
    {
        private readonly ITempDataProvider tempDataProvider;
        private readonly ICompositeViewEngine compositeViewEngine;
        private readonly IServiceProvider serviceProvider;

        public HtmlHelper(ITempDataProvider tempDataProvider, ICompositeViewEngine compositeViewEngine, IServiceProvider serviceProvider)
        {
            this.tempDataProvider = tempDataProvider ?? throw new ArgumentNullException(nameof(tempDataProvider));
            this.compositeViewEngine = compositeViewEngine ?? throw new ArgumentNullException(nameof(compositeViewEngine));
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public async Task<string> RenderRazorViewToString(string viewName, object model)
        {
            using (var sw = new StringWriter())
            {
                var actionContext = GetActionContext();
                IViewEngine viewEngine = compositeViewEngine;
                var viewResult = viewEngine.FindView(actionContext, viewName, false);

                var viewContext = new ViewContext(
                    actionContext,
                    viewResult.View,
                    new ViewDataDictionary(
                        metadataProvider: new EmptyModelMetadataProvider(),
                        modelState: new ModelStateDictionary())
                    {
                        Model = model
                    },
                    new TempDataDictionary(
                        actionContext.HttpContext,
                        tempDataProvider),
                    sw,
                    new HtmlHelperOptions());

                await viewResult.View.RenderAsync(viewContext);
                return sw.GetStringBuilder().ToString();
            }
        }


        private ActionContext GetActionContext()
        {
            var httpContext = new DefaultHttpContext();
            httpContext.RequestServices = serviceProvider;
            return new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
        }
    }
}
