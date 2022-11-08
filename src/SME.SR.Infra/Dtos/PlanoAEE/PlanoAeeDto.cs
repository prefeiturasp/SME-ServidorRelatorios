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