namespace SME.SR.Infra
{
    public class GraficoBarrasVerticalEixoYDto
    {


        public GraficoBarrasVerticalEixoYDto(decimal valorTotalAltura, string titulo, decimal valorTotalEixo, int divisoesDoEixo)
        {
            ValorTotalAltura = valorTotalAltura;
            ValorTotalEixo = valorTotalEixo;
            Titulo = titulo;
            DivisoesDoEixo = divisoesDoEixo;
        }

        public decimal ValorTotalAltura { get; set; }
        public string Titulo { get; set; }        
        public decimal ValorTotalEixo { get; set; }
        public int DivisoesDoEixo { get; set; }
    }
}