using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioPaginadoHistoricoEscolarDto
    {
        public int PaginaAtual { get; set; }
        public IEnumerable<SecaoViewHistoricoEscolar> SecoesPorPagina {  get; set; }
        public HistoricoEscolarDTO HistoricoEscolar { get; set; }
    }
}
