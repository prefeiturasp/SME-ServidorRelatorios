using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class PaginaSubColuna: Pagina
    {
        public Dictionary<SubColuna, List<IColuna>> Colunas { get; set; }
    }
}
