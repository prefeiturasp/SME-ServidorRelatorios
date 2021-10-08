namespace SME.SR.Infra
{
    public class MensagemRelatorioProntoDto
    {
        public MensagemRelatorioProntoDto()
        {
        }

        public MensagemRelatorioProntoDto(string mensagemUsuario, string mensagemTitulo)
        {
            MensagemUsuario = mensagemUsuario;
            MensagemTitulo = mensagemTitulo;
        }

        public string MensagemUsuario { get; set; }
        public string MensagemTitulo { get; set; }
    }
}
