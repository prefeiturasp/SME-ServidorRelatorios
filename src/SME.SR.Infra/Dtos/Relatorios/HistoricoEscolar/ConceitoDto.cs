namespace SME.SR.Infra
{
    public class ConceitoDto
    {
        public string Valor { get; private set; }
        public string Descricao { get; private set; }

        public ConceitoDto(string valor, string descricao)
        {
            Valor = valor;
            Descricao = descricao;
        }
    }
}