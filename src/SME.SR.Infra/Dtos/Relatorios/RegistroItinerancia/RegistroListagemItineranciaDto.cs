using System;
using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RegistroListagemItineranciaDto
    {
        public RegistroListagemItineranciaDto(string dre, string ue, DateTime dataVisita, string objetivos, string alunos, string situacao, string responsavelPaai)
        {
            Dre = dre;
            Ue = ue;
            DataVisita = dataVisita;
            Objetivos = objetivos;
            Alunos = alunos;
            Situacao = situacao;
            ResponsavelPaai = responsavelPaai;
        }

        public string Dre { get; set; }
        public string Ue { get; set; }
        public DateTime DataVisita { get; set; }
        public string Objetivos { get; set; }
        public string Alunos { get; set; }
        public string Situacao { get; set; }
        public string ResponsavelPaai { get; set; }
    }
}
