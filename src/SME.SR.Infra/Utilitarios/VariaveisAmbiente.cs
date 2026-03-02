using Microsoft.Extensions.Configuration;
using System;

namespace SME.SR.Infra
{
    public class VariaveisAmbiente
    {
        private readonly IConfiguration configuration;
        public VariaveisAmbiente(IConfiguration configuration)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public string ConnectionStringSgp => !string.IsNullOrEmpty(configuration.GetSection("ConnectionStrings:SGP_Postgres").Value) ? 
            configuration.GetSection("ConnectionStrings:SGP_Postgres").Value : 
            Environment.GetEnvironmentVariable("ConnectionStrings__SGP_Postgres");

        public string ConnectionStringSgpConsultas => !string.IsNullOrEmpty(configuration.GetSection("ConnectionStrings:SGP_PostgresConsultas").Value) ?
            configuration.GetSection("ConnectionStrings:SGP_PostgresConsultas").Value : 
            Environment.GetEnvironmentVariable("ConnectionStrings__SGP_PostgresConsultas");


        public string ConnectionStringEol => !string.IsNullOrEmpty(configuration.GetSection("EolConnection").Value) ? 
            configuration.GetSection("EolConnection").Value : 
            Environment.GetEnvironmentVariable("EolConnection");

        public string ConnectionStringApiEol => !string.IsNullOrEmpty(configuration.GetSection("ApiEolConnection").Value) ? 
            configuration.GetSection("ApiEolConnection").Value : 
            Environment.GetEnvironmentVariable("ApiEolConnection");

        public string ConnectionStringCoreSso => !string.IsNullOrEmpty(configuration.GetSection("CoreSSOConnection").Value) ? 
            configuration.GetSection("CoreSSOConnection").Value : 
            Environment.GetEnvironmentVariable("CoreSSOConnection");

        public string ConnectionStringSondagem => !string.IsNullOrEmpty(configuration.GetSection("sondagemConnection").Value) ? 
            configuration.GetSection("sondagemConnection").Value : 
            Environment.GetEnvironmentVariable("sondagemConnection");

        public string ConnectionStringAE => !string.IsNullOrEmpty(configuration.GetSection("AEConnection").Value) ? 
            configuration.GetSection("AEConnection").Value : 
            Environment.GetEnvironmentVariable("AEConnection");

        public string ConnectionStringConecta => !string.IsNullOrEmpty(configuration.GetSection("conectaConnection").Value) ?
            configuration.GetSection("conectaConnection").Value :
            Environment.GetEnvironmentVariable("conectaConnection");

        public string PastaArquivosSGP => !string.IsNullOrEmpty(configuration.GetSection("SGPPastaArquivos").Value) ? 
            configuration.GetSection("SGPPastaArquivos").Value : 
            Environment.GetEnvironmentVariable("SGPPastaArquivos");

        public int ProcessamentoMaximoTurmas => !string.IsNullOrEmpty(configuration.GetSection("SR:ProcessamentoMaximoTurmas").Value) ? 
            int.Parse(configuration.GetSection("SR:ProcessamentoMaximoTurmas").Value) : 
            (!String.IsNullOrEmpty(Environment.GetEnvironmentVariable("SR__ProcessamentoMaximoTurmas")) ?
            int.Parse(Environment.GetEnvironmentVariable("SR__ProcessamentoMaximoTurmas")) : 10);

        public int ProcessamentoMaximoUes => !string.IsNullOrEmpty(configuration.GetSection("SR:ProcessamentoMaximoUes").Value) ?
            int.Parse(configuration.GetSection("SR:ProcessamentoMaximoUes").Value) :
            (!String.IsNullOrEmpty(Environment.GetEnvironmentVariable("SR__ProcessamentoMaximoUes")) ?
            int.Parse(Environment.GetEnvironmentVariable("SR__ProcessamentoMaximoUes")) : 10);

        public string ConnectionStringCDEP => !string.IsNullOrEmpty(configuration.GetSection("CDEP_Connection").Value) ?
             configuration.GetSection("CDEP_Connection").Value :
             Environment.GetEnvironmentVariable("CDEP_Connection");

        public string UrlApiNovaSondagem =>
            !string.IsNullOrEmpty(configuration.GetSection("UrlApiNovaSondagem").Value)
                ? configuration.GetSection("UrlApiNovaSondagem").Value
                : Environment.GetEnvironmentVariable("UrlApiNovaSondagem");

        public string ChaveIntegracaoApiSondagem =>
            !string.IsNullOrEmpty(configuration.GetSection("ChaveIntegracaoApiSondagem").Value)
                ? configuration.GetSection("ChaveIntegracaoApiSondagem").Value
                : Environment.GetEnvironmentVariable("ChaveIntegracaoApiSondagem");

        public string UrlApiNovoSgp =>
            !string.IsNullOrEmpty(configuration.GetSection("UrlApiNovoSgp").Value)
                ? configuration.GetSection("UrlApiNovoSgp").Value
                : Environment.GetEnvironmentVariable("UrlApiNovoSgp");

        public string ChaveIntegracaoApiSgp =>
            !string.IsNullOrEmpty(configuration.GetSection("ChaveIntegracaoApiSgp").Value)
                ? configuration.GetSection("ChaveIntegracaoApiSgp").Value
                : Environment.GetEnvironmentVariable("ChaveIntegracaoApiSgp");

    }
}
