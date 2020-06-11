using System;

namespace SME.SR.Infra
{
    public class VariaveisAmbiente
    {
        public string ConnectionStringSgp => Environment.GetEnvironmentVariable("ConnectionStrings__SGP-Postgres");
        public string ConnectionStringEol => Environment.GetEnvironmentVariable("EolConnection");
        public string ConnectionStringApiEol => Environment.GetEnvironmentVariable("ApiEolConnection");
        public string ConnectionStringCoreSso => Environment.GetEnvironmentVariable("CoreSSOConnection");
    }
}
