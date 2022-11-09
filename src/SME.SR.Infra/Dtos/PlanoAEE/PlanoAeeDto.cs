using System;

namespace SME.SR.Infra
{
    public class PlanoAeeDto
    {
        public int VersaoPlano { get; set; }
        public DateTime DataVersaoPlano { get; set; }
        public SituacaoPlanoAee SituacaoPlano { get; set; }
        public string AlunoCodigo { get; set; }
        public string AlunoNome { get; set; }
        public string ParecerCoordenacao { get; set; }
        public long ResponsavelPaaiId { get; set; }
        public string ResponsavelPaaiNome { get; set; }
        public string ResponsavelPaaiLoginRf { get; set; }
        public long ResponsavelId { get; set; }
        public string ResponsavelNome { get; set; }
        public string ResponsavelLoginRf { get; set; }
        public string TurmaNome { get; set; }
        public int AnoLetivo { get; set; }
        public Modalidade Modalidade { get; set; }
        public string UeNome { get; set; }
        public TipoEscola TipoEscola { get; set; }
        public string DreNome { get; set; }
        public string DreAbreviacao { get; set; }
        public string DreGrupo { get; set; }
    }
}