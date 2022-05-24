using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Sentry;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace SME.SR.MVC
{
    public class ExcecaoMiddleware : ExceptionFilterAttribute
    {
        private readonly string sentryDSN;

        public ExcecaoMiddleware(IConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }
            sentryDSN = configuration.GetValue<string>("Sentry:DSN");
        }

        public override void OnException(ExceptionContext context)
        {
            using (SentrySdk.Init(sentryDSN))
            {
                SentrySdk.CaptureException(context.Exception);
            }

            switch (context.Exception)
            {
                case NegocioException negocioException:
                    context.HttpContext.Response.ContentType = "application/json";
                    var erros = new
                    {
                        Mensagens = new List<string> { negocioException.Message },
                    };
                    var json = JsonConvert.SerializeObject(erros);
                    context.Result = new ObjectResult(JsonConvert.SerializeObject(json));
                    Console.WriteLine(json);
                    break;
                default:
                    Console.WriteLine(context.Exception.Message);
                    context.Result = new ObjectResult($"Ocorreu um erro interno. {ex}");
                    break;
            }

            base.OnException(context);
        }
    }
}
