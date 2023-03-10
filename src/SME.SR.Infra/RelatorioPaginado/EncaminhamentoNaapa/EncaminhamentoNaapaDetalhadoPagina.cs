using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class EncaminhamentoNaapaDetalhadoPagina
    {
        public CabecalhoEncaminhamentoNAAPADetalhadoDto Cabecalho { get; set; }
        public List<SecaoRelatorioEncaminhamentoNaapa> Linhas { get; set; }
        public int Pagina { get; set; }
    }
}
