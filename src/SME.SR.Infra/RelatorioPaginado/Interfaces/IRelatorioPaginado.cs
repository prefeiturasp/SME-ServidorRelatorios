using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra.RelatorioPaginado.Interfaces
{
    public interface IRelatorioPaginado
    {
        List<Pagina> Paginas();
    }
}
