﻿using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class RelatorioSondagemComponentesPorTurmaCabecalhoDto
    {
        public string Dre { get; set; }
        public string Ue { get; set; }
        public int AnoLetivo { get; set; }
        public int Ano { get; set; }
        public string Turma { get; set; }
        public string ComponenteCurricular { get; set; }
        public string Proficiencia { get; set; }
        public string Periodo { get; set; }
        public string Usuario { get; set; }
        public string Rf { get; set; }
        public DateTime DataSolicitacao { get; set; }
        public List<RelatorioSondagemComponentesPorTurmaOrdemDto> Ordens { get; set; }

        public List<RelatorioSondagemComponentesPorTurmaPerguntaDto> Perguntas { get; set; }

        public RelatorioSondagemComponentesPorTurmaCabecalhoDto()
        {
            Ordens = new List<RelatorioSondagemComponentesPorTurmaOrdemDto>();
            Perguntas = new List<RelatorioSondagemComponentesPorTurmaPerguntaDto>();
        }
    }
}
