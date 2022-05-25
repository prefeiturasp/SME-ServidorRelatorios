using Elastic.Apm;
using Microsoft.ApplicationInsights;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SME.SR.Infra
{
    public class ServicoTelemetria : IServicoTelemetria
    {
        private readonly TelemetryClient insightsClient;
        private readonly TelemetriaOptions telemetriaOptions;

        public ServicoTelemetria(TelemetryClient insightsClient, TelemetriaOptions telemetriaOptions)
        {
            this.insightsClient = insightsClient;
            this.telemetriaOptions = telemetriaOptions ?? throw new ArgumentNullException(nameof(telemetriaOptions));
        }

        public async Task<dynamic> RegistrarComRetornoAsync<T>(Func<Task<object>> acao, string acaoNome, string telemetriaNome, string telemetriaValor, string parametros = "")
        {
            DateTime inicioOperacao = default;
            Stopwatch temporizador = default;

            dynamic result = default;

            if (telemetriaOptions.ApplicationInsights)
            {
                inicioOperacao = DateTime.UtcNow;
                temporizador = Stopwatch.StartNew();
            }

            if (telemetriaOptions.Apm)
            {
                var transactionElk = Agent.Tracer.CurrentTransaction;

                await transactionElk.CaptureSpan(telemetriaNome, "db", async (span) =>
                {
                    span.SetLabel(telemetriaNome, telemetriaValor);
                    span.SetLabel("parametros", parametros);

                    result = await acao();
                }, "postgresql", acaoNome);
            }
            else
            {
                result = await acao();
            }

            if (telemetriaOptions.ApplicationInsights)
            {
                temporizador.Stop();

                insightsClient?.TrackDependency(acaoNome, telemetriaNome, telemetriaValor, inicioOperacao, temporizador.Elapsed, true);
            }

            return result;
        }

        public dynamic RegistrarComRetorno<T>(Func<object> acao, string acaoNome, string telemetriaNome, string telemetriaValor, string parametros = "")
        {
            return RegistrarComRetorno<T>(acao, acaoNome, "db", "postgresql", telemetriaNome, telemetriaValor, parametros);
        }

        public void Registrar(Action acao, string acaoNome, string telemetriaNome, string telemetriaValor)
        {
            Registrar(acao, acaoNome, "db", "postgresql", telemetriaNome, telemetriaValor);
        }

        public async Task RegistrarAsync(Func<Task> acao, string acaoNome, string telemetriaNome, string telemetriaValor, string parametros = "")
        {
            DateTime inicioOperacao = default;
            Stopwatch temporizador = default;

            if (telemetriaOptions.ApplicationInsights)
            {
                inicioOperacao = DateTime.UtcNow;
                temporizador = Stopwatch.StartNew();
            }

            if (telemetriaOptions.Apm)
            {
                var transactionElk = Agent.Tracer.CurrentTransaction;

                await transactionElk.CaptureSpan(telemetriaNome, "http", async (span) =>
                {
                    span.SetLabel(telemetriaNome, telemetriaValor);
                    span.SetLabel("parametros", parametros);

                    await acao();
                }, "http", acaoNome);
            }
            else
            {
                await acao();
            }

            if (telemetriaOptions.ApplicationInsights)
            {
                temporizador.Stop();
                insightsClient?.TrackDependency(acaoNome, telemetriaNome, telemetriaValor, inicioOperacao, temporizador.Elapsed, true);
            }
        }

        public dynamic RegistrarComRetorno<T>(Func<object> acao, string acaoNome, string tipo, string subTipo, string telemetriaNome, string telemetriaValor, string parametros = "")
        {
            DateTime inicioOperacao = default;
            Stopwatch temporizador = default;

            dynamic result = default;

            if (telemetriaOptions.ApplicationInsights)
            {
                inicioOperacao = DateTime.UtcNow;
                temporizador = Stopwatch.StartNew();
            }

            if (telemetriaOptions.Apm)
            {
                var transactionElk = Agent.Tracer.CurrentTransaction;

                transactionElk.CaptureSpan(telemetriaNome, tipo, (span) =>
                {
                    span.SetLabel(telemetriaNome, telemetriaValor);
                    span.SetLabel("parametros", parametros);

                    result = acao();
                }, subTipo, acaoNome);
            }
            else
            {
                result = acao();
            }

            if (telemetriaOptions.ApplicationInsights)
            {
                temporizador.Stop();

                insightsClient?.TrackDependency(acaoNome, telemetriaNome, telemetriaValor, inicioOperacao, temporizador.Elapsed, true);
            }

            return result;
        }

        public void Registrar(Action acao, string acaoNome, string tipo, string subTipo, string telemetriaNome, string telemetriaValor)
        {
            DateTime inicioOperacao = default;
            Stopwatch temporizador = default;

            if (telemetriaOptions.ApplicationInsights)
            {
                inicioOperacao = DateTime.UtcNow;
                temporizador = Stopwatch.StartNew();
            }

            if (telemetriaOptions.Apm)
            {
                var transactionElk = Agent.Tracer.CurrentTransaction;

                transactionElk.CaptureSpan(telemetriaNome, tipo, (span) =>
                {
                    span.SetLabel(telemetriaNome, telemetriaValor);
                    acao();
                }, subTipo, acaoNome);
            }
            else
            {
                acao();
            }

            if (telemetriaOptions.ApplicationInsights)
            {
                temporizador.Stop();
                insightsClient?.TrackDependency(acaoNome, telemetriaNome, telemetriaValor, inicioOperacao, temporizador.Elapsed, true);
            }
        }
    }
}