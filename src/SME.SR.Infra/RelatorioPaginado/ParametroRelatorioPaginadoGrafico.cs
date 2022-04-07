using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class ParametroRelatorioPaginadoGrafico
    {
        public List<GraficoBarrasVerticalDto> Graficos {  get; set; }

        public int TotalDeGraficosPorLinha {  get; set; }

        public int TotalDeLinhas { get; set; }  
    }
}
