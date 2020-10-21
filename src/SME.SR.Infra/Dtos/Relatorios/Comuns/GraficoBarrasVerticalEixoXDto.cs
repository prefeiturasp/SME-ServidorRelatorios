namespace SME.SR.Infra
{
    public class GraficoBarrasVerticalEixoXDto
    {
        public GraficoBarrasVerticalEixoXDto(decimal valor, string titulo)
        {
            Valor = valor;
            Titulo = titulo;
        }

        public decimal Valor { get; set; }
        public string Titulo { get; set; }
    }
}