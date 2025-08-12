using SME.SR.Infra.CDEP;

namespace SME.SR.Infra.Dtos.Relatorios.CDEP
{
    public class ControleAcervoDTO
    {
        public string Tombo { get; set; }
        public string Titulo { get; set; }
        public TipoAcervo TipoAcervo { get; set; }
        public SituacaoEmprestimo SituacaoEmprestimo { get; set; }
        public int? QuantidadeEmprestimos { get; set; }
    }
}
