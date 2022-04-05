using SME.SR.Infra.RelatorioPaginado.Interfaces;
using System.Collections;
using System.Collections.Generic;

namespace SME.SR.Infra.RelatorioPaginado
{
    public class Pagina
    {
        public int Indice { get; set; }

        public int Ordenacao { get; set; }

        public List<IColuna> Colunas { get; set; }

        public IList Valores { get; set; }
    }
}
