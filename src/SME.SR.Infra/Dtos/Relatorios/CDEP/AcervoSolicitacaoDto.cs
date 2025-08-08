using SME.SR.Infra.CDEP;
using System;

namespace SME.SR.Infra.Dtos.Relatorios.CDEP
{
    public class AcervoSolicitacaoDto
    {
        public string Tombo { get; set; }
        public string Titulo { get; set; }
        public string Solicitante { get; set; }
        public SituacaoEmprestimo SituacaoEmprestimo { get; set; }
        public int? QuantidadeEmprestimos { get; set; }
        public DateTime DataEmprestimo { get; set; }
        public DateTime DataDevolucao { get; set; }
    }
}
