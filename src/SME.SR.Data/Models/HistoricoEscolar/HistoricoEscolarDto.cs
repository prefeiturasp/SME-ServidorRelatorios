using System.Collections.Generic;

namespace SME.SR.Data
{
    public partial class HistoricoEscolarDTO
    {
        public string nomeDre { get; set; }
        public CabecalhoDTO cabecalho { get; set; }
        public InformacoesAlunoDTO informacoesAluno { get; set; }
        public List<CicloDTO> ciclos { get; set; }
        public List<GruposComponentesCurricularesDTO> gruposComponentesCurriculares { get; set; }
        public EnsinoReligiosoDTO ensinoReligioso { get; set; }
        public List<EnriquecimentoCurricularDTO> enriquecimentoCurricular { get; set; }
        public List<ParecerConclusivoDTO> parecerConclusivo { get; set; }
        public LegendaDTO legenda { get; set; }
    }
}
