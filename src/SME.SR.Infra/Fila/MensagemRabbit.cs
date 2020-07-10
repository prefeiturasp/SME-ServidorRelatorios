using System;

namespace SME.SR.Infra
{
    public class MensagemRabbit
    {
        public MensagemRabbit(string action, object mensagem, Guid codigoCorrelacao, string usuarioLogadoRF = null)
        {
            Action = action;
            Mensagem = mensagem;
            CodigoCorrelacao = codigoCorrelacao;
            UsuarioLogadoRF = usuarioLogadoRF;
        }

        public string Action { get; set; }
        public object Mensagem { get; set; }
        public Guid CodigoCorrelacao { get; set; }
        public string UsuarioLogadoRF { get; set; }

    }
}
