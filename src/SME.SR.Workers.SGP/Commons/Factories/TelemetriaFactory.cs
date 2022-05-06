using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SME.SR.Infra;

namespace SME.SR.Workers.SGP.Commons.Factories
{
    public class TelemetriaFactory
    {
        public IServicoTelemetria GetServicoTelemetria(IServiceCollection services, IConfiguration configuration)
        {
            var serviceProvider = services.BuildServiceProvider();
            var clientTelemetry = serviceProvider.GetService<TelemetryClient>();

            var telemetriaOptions = new TelemetriaOptions();
            configuration.GetSection(TelemetriaOptions.Secao).Bind(telemetriaOptions, c => c.BindNonPublicProperties = true);

            return new ServicoTelemetria(clientTelemetry, telemetriaOptions);
        }
    }
}
