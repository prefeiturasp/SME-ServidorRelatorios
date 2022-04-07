using System.Collections.Generic;
using System.Linq;

namespace SME.SR.Infra
{
    public class RelatorioPaginadoDto
    {
        public string ViewCustomParametroCabecalho { get; set; } = "ParametroCabecalhoDefault.cshtml";

        public string ViewCustomConteudo { get; set; } = "ConteudoDefault.cshtml";

        public string ViewCustomCss { get; set; } = "CssDefault.cshtml";

        public CabecalhoPaginadoDto Cabecalho { get; set; }

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
