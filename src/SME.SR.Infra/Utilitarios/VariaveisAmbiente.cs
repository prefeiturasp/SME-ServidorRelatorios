using System;

namespace SME.SR.Infra
{
    public class VariaveisAmbiente
    {

        public string ConnectionStringSgp => Environment.GetEnvironmentVariable("ConnectionStrings__SGP_Postgres");        
        public string ConnectionStringEol => Environment.GetEnvironmentVariable("EolConnection");
        public string ConnectionStringApiEol => Environment.GetEnvironmentVariable("ApiEolConnection");
        public string ConnectionStringCoreSso => Environment.GetEnvironmentVariable("CoreSSOConnection");
        public string ConnectionStringSondagem => Environment.GetEnvironmentVariable("sondagemConnection");
        public string ConnectionStringAE => Environment.GetEnvironmentVariable("AEConnection");
        public string PastaArquivosSGP => Environment.GetEnvironmentVariable("SGPPastaArquivos");
        public int ProcessamentoMaximoTurmas => !String.IsNullOrEmpty(Environment.GetEnvironmentVariable("SR__ProcessamentoMaximoTurmas")) ? 
            int.Parse(Environment.GetEnvironmentVariable("SR__ProcessamentoMaximoTurmas")) : 10;
    }
}
