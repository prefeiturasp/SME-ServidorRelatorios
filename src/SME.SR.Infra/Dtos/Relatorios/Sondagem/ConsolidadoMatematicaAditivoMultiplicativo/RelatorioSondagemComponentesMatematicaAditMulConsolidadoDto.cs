using System.Collections.Generic;
using System.Linq;

namespace SME.SR.Infra
{
    public class RelatorioSondagemComponentesMatematicaAditMulConsolidadoDto
    {
        public RelatorioSondagemComponentesMatematicaAditMulConsolidadoDto()
        {
            PerguntasRespostas = new List<RelatorioSondagemComponentesMatematicaAditMulConsolidadoPerguntasRespostasDto>();
            Perguntas = new List<RelatorioSondagemComponentesMatematicaAditMulConsolidadoPerguntaDto>();
            GraficosBarras = new List<GraficoBarrasVerticalDto>();
        }
        public string Ano { get; set; }
        public int AnoLetivo { get; set; }
        public string ComponenteCurricular { get; set; }
        public string DataSolicitacao { get; set; }
        public string Dre { get; set; }
        public string Ue { get; set; }
        public string Periodo { get; set; }
        public string Proficiencia { get; set; }
        public string RF { get; set; }
        public string Usuario { get; set; }
        public string Turma { get; set; }
        public int TotalDeAlunos { get; set; }
        public List<RelatorioSondagemComponentesMatematicaAditMulConsolidadoPerguntaDto> Perguntas { get; set; }

        public List<RelatorioSondagemComponentesMatematicaAditMulConsolidadoPerguntasRespostasDto> PerguntasRespostas { get; set; }

        public List<GraficoBarrasVerticalDto> GraficosBarras { get; set; }

        public string[] ObterPerguntasPorId(long[] ids)
        {
            return Perguntas.Where(a => ids.Contains(a.Id)).Select(a => a.Descricao).ToArray();
        }
    }
}
