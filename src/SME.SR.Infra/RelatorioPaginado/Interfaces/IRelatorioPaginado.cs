using System.Collections.Generic;

namespace SME.SR.Infra
{
    public interface IRelatorioPaginado
    {
        List<Pagina> Paginas();
    }
}
