using System.Collections.Generic;
using System.Linq;

namespace SME.SR.Infra
{
    public class RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoPerguntasRespostasDto
    {
        public RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoPerguntasRespostasDto()
        {
            Respostas = new List<RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoRespostaDto>();
        }
        public string Pergunta { get; set; }
        
        public double PercentualTotal { get { return Respostas.Sum(a => a.AlunosPercentual); } }
        public double AlunosTotal { get { return Respostas.Sum(a => a.AlunosQuantidade); }  }

        public List<RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoRespostaDto> Respostas  { get; set; }
    }
}