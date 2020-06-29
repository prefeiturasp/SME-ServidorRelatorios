using System;

namespace SME.SR.Infra
{
    public class VariaveisAmbiente
    {

        public string ConnectionStringSgp => "User ID=postgres;Password=postgres;Host=10.50.0.196;Port=5432;Database=sgp_db;Pooling=true";// Environment.GetEnvironmentVariable("ConnectionStrings__SGP-Postgres");        
        public string ConnectionStringEol => Environment.GetEnvironmentVariable("EolConnection");
        public string ConnectionStringApiEol => Environment.GetEnvironmentVariable("ApiEolConnection");
        public string ConnectionStringCoreSso => Environment.GetEnvironmentVariable("CoreSSOConnection");
    }
}
