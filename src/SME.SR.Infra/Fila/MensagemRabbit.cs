using System;

namespace SME.SR.Infra
{
    public class MensagemRabbit
    {
        public MensagemRabbit(string action, object filtros, Guid codigoCorrelacao)
        {
            Action = action;
            Filtros = filtros;
            CodigoCorrelacao = codigoCorrelacao;
        }

        public string Action { get; set; }
        public object Filtros { get; set; }
        public Guid CodigoCorrelacao { get; set; }
    }
}
