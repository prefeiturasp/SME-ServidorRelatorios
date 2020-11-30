namespace SME.SR.Infra
{
    public class AlunosAlteracaoNotasDto
    {
        public string NumeroChamada { get; set; }
        public string Nome { get; set; }
        public TipoAlteracaoNota TipoNota { get; set; }
        public string NotaAnterior { get; set; }
        public string NotaAtribuida { get; set; }
        public string DataAlteracao { get; set; }
        public string UsuarioAlteracao { get; set; }
        public WorkflowAprovacaoNivelStatus Situacao { get; set; }
        public string UsuarioAprovacao { get; set; }
        public TipoNota TipoNotaConceito { get; set; }
        public TipoConceito ConceitoAnterior { get; set; }
        public TipoConceito ConceitoAtribuido { get; set; }
    }
}
