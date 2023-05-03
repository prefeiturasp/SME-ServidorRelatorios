using System;
using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class ListagemItineranciaDto
    {
        public ListagemItineranciaDto()
        {}

        public long Id { get; set; }
        public DateTime DataVisita { get; set; }
        public SituacaoItinerancia Situacao { get; set; }
        public int AnoLetivo { get; set; }
        public string ResponsavelPaaiNome { get; set; }
        public string ResponsavelPaaiLoginRf { get; set; }
        public string UeCodigo { get; set; }
        public string UeNome { get; set; }
        public TipoEscola TipoEscola { get; set; }
        public string DreCodigo { get; set; }
        public string DreNome { get; set; }
        public string DreAbreviacao { get; set; }

        public List<ObjetivoItineranciaDto> Objetivos = new List<ObjetivoItineranciaDto>();
        public List<AlunoItineranciaDto> Alunos = new List<AlunoItineranciaDto>();
    }
}
