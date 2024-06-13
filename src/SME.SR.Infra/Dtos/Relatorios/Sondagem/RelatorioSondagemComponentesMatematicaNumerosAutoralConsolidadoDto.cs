using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoDto
    {
        public RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoDto()
        {
            PerguntasRespostas = new List<RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoPerguntasRespostasDto>();
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
        public List<RelatorioSondagemComponentesMatematicaNumerosAutoralConsolidadoPerguntasRespostasDto> PerguntasRespostas { get; set; }
        public List<GraficoBarrasVerticalDto> GraficosBarras { get; set; }
    }
}
