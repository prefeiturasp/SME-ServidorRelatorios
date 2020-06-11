namespace SME.SR.Infra
{
    public class PublicaFilaDto
    {
        public PublicaFilaDto(object dados, string nomeFila, string rota, string exchange = null)
        {
            Dados = dados;
            
            NomeFila = nomeFila;
            Rota = rota;
            if (!string.IsNullOrWhiteSpace(exchange))
                Exchange = exchange;
        }

        public string NomeFila { get; set; }
        public object Dados { get; set; }
        public string Rota { get; }
        public string Exchange { get; set; }
    }
}
