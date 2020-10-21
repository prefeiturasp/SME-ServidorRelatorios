namespace SME.SR.Infra
{
    public class GraficoBarrasVerticalEixoYDto
    {
        public GraficoBarrasVerticalEixoYDto(decimal valor, string titulo)
        {
            Valor = valor;
            Titulo = titulo;
        }

        public decimal Valor { get; set; }
        public string Titulo { get; set; }
    }
}