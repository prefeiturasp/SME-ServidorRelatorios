using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;

namespace SME.SR.Workers.SGP.Filters
{
    public class FiltroIntegracao: IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {

            var attributes = context.MethodInfo.DeclaringType.GetCustomAttributes(true)
                                    .Union(context.MethodInfo.GetCustomAttributes(true))
                                    .OfType<ChaveIntegracaoSrApi>();

            if (attributes != null && attributes.Any())
            {

                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = "x-sr-api-key",
                    In = ParameterLocation.Header,
                    Description = "Chave de Integração da API do Servidor de Relatórios",
                    Required = true
                });
            }
        }
    }
}
