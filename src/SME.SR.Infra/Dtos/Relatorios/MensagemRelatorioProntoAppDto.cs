namespace SME.SR.Infra
{
    public class MensagemRelatorioProntoAppDto
    {
        public MensagemRelatorioProntoAppDto(string mensagemUsuario, string mensagemTitulo, string mensagemDados)
        {
            MensagemUsuario = mensagemUsuario;
            MensagemTitulo = mensagemTitulo;
            MensagemDados = mensagemDados;
        }

        public string MensagemUsuario { get; set; }
        public string MensagemTitulo { get; set; }
        public string MensagemDados { get; set; }
    }
}
