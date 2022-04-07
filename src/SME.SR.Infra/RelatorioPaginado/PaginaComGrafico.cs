using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra.RelatorioPaginado
{
    public class PaginaComGrafico: Pagina
    {
        public List<List<GraficoBarrasVerticalDto>> Graficos { get; set; }
    }
}
