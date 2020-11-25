namespace SME.SR.Infra
{
    public class GraficoBarrasPAPVerticalEixoXDto
    {
        public GraficoBarrasPAPVerticalEixoXDto(decimal valor, decimal porcentagem, string titulo)
        {
            Valor = valor;
            Porcentagem = porcentagem;
            Titulo = titulo;
        }

        public decimal Porcentagem { get; set; }
        public decimal Valor { get; set; }
        public string Titulo { get; set; }
    }
}
