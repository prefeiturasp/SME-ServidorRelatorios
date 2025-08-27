using SME.SR.Infra.CDEP;

namespace SME.SR.Infra.Dtos.Relatorios.CDEP
{
    public class ControleEditoraDTO
    {
        public string Tombo { get; set; }
        public string Titulo { get; set; }
        public string Editora { get; set; }
        public SituacaoEmprestimo SituacaoEmprestimo { get; set; }
    }
}
