namespace SME.SR.Infra
{
    public class MensagemRelatorioProntoDto
    {
        public MensagemRelatorioProntoDto()
        {
        }

        public MensagemRelatorioProntoDto(string mensagemUsuario, string mensagemTitulo, string mensagemDados = null)
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
