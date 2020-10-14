using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class RelatorioSondagemPortuguesPorTurmaCabecalhoDto
    {
        public string Dre { get; set; }
        public string Ue { get; set; }
        public int AnoLetivo { get; set; }
        public string Ano { get; set; }
        public string Turma { get; set; }
        public string ComponenteCurricular { get; set; }
        public string Proficiencia { get; set; }
        public string Periodo { get; set; }
        public string Usuario { get; set; }
        public string Rf { get; set; }
        public string DataSolicitacao { get; set; }
        public List<RelatorioSondagemComponentesPorTurmaPerguntaDto> Perguntas { get; set; }

        public RelatorioSondagemPortuguesPorTurmaCabecalhoDto()
        {
            Perguntas = new List<RelatorioSondagemComponentesPorTurmaPerguntaDto>();
        }
    }
}
