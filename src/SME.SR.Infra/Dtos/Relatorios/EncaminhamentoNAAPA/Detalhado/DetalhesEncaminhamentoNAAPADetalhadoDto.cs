using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class DetalhesEncaminhamentoNAAPADetalhadoDto
    {
        public SecaoQuestoesEncaminhamentoNAAPADetalhadoDto Informacoes { get; set; }
        public SecaoQuestoesEncaminhamentoNAAPADetalhadoDto QuestoesApresentadas { get; set; }
        public IEnumerable<SecaoQuestoesEncaminhamentoNAAPAItineranciaDetalhadoDto> Itinerancia { get; set; }
    }
}
