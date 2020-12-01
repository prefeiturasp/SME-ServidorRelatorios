namespace SME.SR.Infra
{
    public class AlunosAlteracaoNotasDto
    {
        public string NumeroChamada { get; set; }
        public string Nome { get; set; }
        public string TipoAlteracaoNota { get; set; }
        public string NotaConceitoAnterior { get; set; }
        public string NotaConceitoAtribuido { get; set; }
        public string DataAlteracao { get; set; }
        public string UsuarioAlteracao { get; set; }
        public string Situacao { get; set; }
        public string UsuarioAprovacao { get; set; }        
        
    }
}
