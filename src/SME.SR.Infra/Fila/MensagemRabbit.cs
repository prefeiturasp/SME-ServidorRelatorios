namespace SME.SR.Infra
{
    public class MensagemRabbit
    {
        public MensagemRabbit(string action, object filtros)
        {
            Action = action;
            Filtros = filtros;
        }

        public string Action { get; set; }
        public object Filtros { get; set; }
    }
}
