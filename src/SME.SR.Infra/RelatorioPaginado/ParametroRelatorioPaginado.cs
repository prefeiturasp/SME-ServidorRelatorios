using System.Collections.Generic;

namespace SME.SR.Infra
{
    public abstract class ParametroRelatorioPaginado<T> where T : class
    {
        public List<T> Valores { get; set; }
    }
}
