using System.Collections.Generic;
using System.Linq;

namespace SME.SR.Infra
{
    public class RelatorioSondagemComponentesMatematicaAditMulConsolidadoPerguntasRespostasDto
    {
        public RelatorioSondagemComponentesMatematicaAditMulConsolidadoPerguntasRespostasDto()
        {
            Respostas = new List<RelatorioSondagemComponentesMatematicaAditMulConsolidadoRespostaDto>();
        }
        public string Ordem { get; set; }
        public List<RelatorioSondagemComponentesMatematicaAditMulConsolidadoRespostaDto> Respostas { get; set; }

        public long[] ObterIdsDasPerguntas()
        {
            return Respostas.Select(a => a.PerguntaId).Distinct().ToArray();
        }

        public string[] ObterIdsDasPerguntasNovas()
        {
            return Respostas.Select(a => a.PerguntaNovaId).Distinct().ToArray();
        }
    }
}
