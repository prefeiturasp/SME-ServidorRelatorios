using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SME.SR.Workers.SGP.Commons.Config
{
    public static class ConnectionStrings
    {
        public static string ConexaoEol = Environment.GetEnvironmentVariable("EolConnection");
        public static string ConexaoSgp = Environment.GetEnvironmentVariable("SgpConnection");
        public static string ConexaoApiEol = Environment.GetEnvironmentVariable("EolApiConnection");
    }
}
