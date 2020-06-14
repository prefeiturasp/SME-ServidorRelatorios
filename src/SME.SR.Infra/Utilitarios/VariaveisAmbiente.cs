using System;

namespace SME.SR.Infra
{
    public class VariaveisAmbiente
    {
        //TODO REMOVER MODIFICAR O QUANTO ANTES
        //public string ConnectionStringSgp => Environment.GetEnvironmentVariable("ConnectionStrings__SGP-Postgres");
        public string ConnectionStringSgp => "User ID=postgres;Password=postgres;Host=10.50.1.33;Port=30808;Database=sgp_db-dev;Pooling=true;";
        public string ConnectionStringEol => Environment.GetEnvironmentVariable("EolConnection") + "Encrypt=False;TrustServerCertificate=True;";
        public string ConnectionStringApiEol => Environment.GetEnvironmentVariable("ApiEolConnection");
        public string ConnectionStringCoreSso => Environment.GetEnvironmentVariable("CoreSSOConnection");
    }
}
