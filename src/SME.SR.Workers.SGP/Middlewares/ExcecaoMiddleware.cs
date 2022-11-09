using MediatR;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Sentry;
using SME.SR.Application;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace SME.SR.Workers.SGP.Middlewares
{
    public class ExcecaoMiddleware : IMiddleware
    {

        private readonly IMediator mediator;

        public ExcecaoMiddleware(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (NegocioException nex)
            {
                await RetornarErro(context, nex.Message, 601);
                await RegistraErro(nex.Message, LogNivel.Negocio);
            }
            catch (Exception ex)
            {
                await RetornarErro(context, "Ocorreu um erro interno." + ex.Message, (int)HttpStatusCode.InternalServerError);
                await RegistraErro(ex.Message, LogNivel.Critico);
            }
        }

        private Task RegistraErro(string message, LogNivel nivel)
            => mediator.Send(new SalvarLogViaRabbitCommand(message, nivel));

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
