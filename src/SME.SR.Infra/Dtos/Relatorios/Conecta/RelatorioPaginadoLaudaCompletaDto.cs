using System.Collections.Generic;
using System.Linq;

namespace SME.SR.Infra.Dtos.Relatorios.Conecta
{
    public class RelatorioPaginadoLaudaCompletaDto
    {
        public RelatorioPaginadoLaudaCompletaDto()
        {
            Paginas = new List<RelatorioPaginaLaudaCompletaDto>();
        }

        public int TotalPagina { get { return Paginas.Count(); } }
        public List<RelatorioPaginaLaudaCompletaDto> Paginas { get; set; }

        public void AdicionarPagina(RelatorioPaginaLaudaCompletaDto pagina)
        {
            Paginas.Add(pagina);
        }
    }
}
