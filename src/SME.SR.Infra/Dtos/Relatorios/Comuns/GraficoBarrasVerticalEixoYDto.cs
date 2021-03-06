﻿namespace SME.SR.Infra
{
    public class GraficoBarrasVerticalEixoYDto
    {


        public GraficoBarrasVerticalEixoYDto(decimal valorTotalAltura, string titulo, int valorMaximoEixo, int divisoesDoEixo)
        {
            ValorTotalAltura = valorTotalAltura;
            ValorMaximoEixo = valorMaximoEixo;
            Titulo = titulo;
            DivisoesDoEixo = divisoesDoEixo;
        }

        public decimal ValorTotalAltura { get; set; }
        public string Titulo { get; set; }        
        public int ValorMaximoEixo { get; set; }
        public int DivisoesDoEixo { get; set; }

        public decimal ValorIncrementoDivisao { get { return ValorMaximoEixo / DivisoesDoEixo; } }

        public decimal ValorDecrescimentoAlturaDivisao { get { return ValorTotalAltura / DivisoesDoEixo; } }

    }
}