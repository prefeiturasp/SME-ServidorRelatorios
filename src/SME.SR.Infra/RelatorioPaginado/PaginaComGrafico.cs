using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class PaginaComGrafico: Pagina
    {
        public List<List<GraficoBarrasVerticalDto>> Graficos { get; set; }
    }
}
