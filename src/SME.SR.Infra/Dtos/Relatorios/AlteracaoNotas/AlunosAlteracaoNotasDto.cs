namespace SME.SR.Infra
{
    public class AlunosAlteracaoNotasDto
    {
        public long Numero { get; set; }
        public string Nome { get; set; }
        public string TipoNota { get; set; }
        public string NotaAnterior { get; set; }
        public string NotaAtribuida { get; set; }
        public string DataAlteracao { get; set; }
        public string UsuarioAlteracao { get; set; }
        public string Situacao { get; set; }
        public string UsuarioAprovacao { get; set; }
    }
}
