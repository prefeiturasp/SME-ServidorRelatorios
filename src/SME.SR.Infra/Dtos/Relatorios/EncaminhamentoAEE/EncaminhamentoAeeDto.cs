using System;

namespace SME.SR.Infra
{
    public class EncaminhamentoAeeDto
    {
        public long Id { get; set; }    
        public long DreId { get; set; }
        public string DreAbreviacao { get; set; }
        public string UeCodigo { get; set; }
        public string UeNome { get; set; }
        public TipoEscola TipoEscola { get; set; }
        public string AlunoCodigo { get; set; }
        public string AlunoNome { get; set; }
        public int Situacao { get; set; }
        public string TurmaCodigo { get; set; }
        public string TurmaNome { get; set; }
        public Modalidade Modalidade { get; set; }
        public string ResponsavelPaaiNome { get; set; }
        public string ResponsavelPaaiLoginRf { get; set; }
        public DateTime CriadoEm { get; set; }
        public int AnoLetivo { get; set; }
    }
}
