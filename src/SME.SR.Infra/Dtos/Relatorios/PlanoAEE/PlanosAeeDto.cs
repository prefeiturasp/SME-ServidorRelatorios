using System;

namespace SME.SR.Infra
{
    public class PlanosAeeDto
    {
        public string DreNome { get; set; }
        public string DreAbreviacao { get; set; }
        public string UeCodigo { get; set; }
        public string UeNome { get; set; }
        public TipoEscola TipoEscola { get; set; }
        public string AlunoCodigo { get; set; }
        public string AlunoNome { get; set; }
        public string TurmaCodigo { get; set; }
        public string TurmaNome { get; set; }
        public int AnoLetivo { get; set; }
        public Modalidade Modalidade { get; set; }
        public int SituacaoPlano { get; set; }
        public string ResponsavelNome { get; set; }
        public string ResponsavelLoginRf { get; set; }
        public int VersaoPlano { get; set; }
        public DateTime DataVersaoPlano { get; set; }
        public string ResponsavelPaaiNome { get; set; }
        public string ResponsavelPaaiLoginRf { get; set; }
    }
}