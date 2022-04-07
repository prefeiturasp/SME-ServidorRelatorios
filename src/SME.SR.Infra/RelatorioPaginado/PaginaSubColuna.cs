using SME.SR.Infra.RelatorioPaginado.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra.RelatorioPaginado
{
    public class PaginaSubColuna: Pagina
    {
        public Dictionary<SubColuna, List<IColuna>> Colunas { get; set; }
    }
}
