using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Sentry;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace SME.SR.Workers.SGP.Middlewares
{
    public class ExcecaoMiddleware : IMiddleware
    {

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (NegocioException nex)
            {
                await RetornarErro(context, nex.Message, 601);
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                await RetornarErro(context, $"Ocorreu um erro interno. {ex}", (int)HttpStatusCode.InternalServerError);
            }
        }
        private static Task RetornarErro(HttpContext context, string mensagem, int statusCode)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            var json = new
            {
                Mensagens = new List<string> { mensagem },
            };

            return context.Response.WriteAsync(JsonConvert.SerializeObject(json));
        }
    }
}
