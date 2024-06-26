﻿namespace SME.SR.Infra
{
    public class ParametroRelatorioPaginadoQuantitativo<T> : ParametroRelatorioPaginado<T> where T : class
    {
        public int QuantidadeDeLinhas { get; set; }

        public int QuantidadeDeColunasPorLinha { get; set; }

        public int QuantidadeTotalDeColunas { get; set; }
    }
}
