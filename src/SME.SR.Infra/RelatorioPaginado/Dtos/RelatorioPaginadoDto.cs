using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace SME.SR.Infra.RelatorioPaginado.Dtos
{
    public class RelatorioPaginadoDto
    {
        public string ViewCustomParametroCabecalho { get; set; } = "ParametroCabecalhoDefault";

        public string ViewCustomConteudo { get; set; } = "ConteudoDefault";

        public CabecalhoDto Cabecalho { get; set; }

        public List<Pagina> Paginas { get; set; }

        public int TotalDePagina 
        {  
            get
            {
                if (this.Paginas != null && this.Paginas.Count > 0)
                {
                    return this.Paginas.Max(pagina => pagina.Indice);
                }

                return 0;
                
            } 
        }
    }
}
