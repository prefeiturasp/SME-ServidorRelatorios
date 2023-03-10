using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioPaginadoHistoricoEscolarDto
    {
        public int Pagina { get; set; }
        public List<SecaoViewHistoricoEscolar> SecoesPorPagina {  get; set; }
        public HistoricoEscolarDTO HistoricoEscolar { get; set; }
    }
}
