using System;
using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioFrequenciaIndividualAlunosDto
    {
        public RelatorioFrequenciaIndividualAlunosDto()
        {
            Bimestres = new List<RelatorioFrequenciaIndividualBimestresDto>();
        }
        public string NomeAluno { get; set; }
        public string CodigoAluno { get; set; }
        public int TotalAulasDadasFinal { get; set; }
        public int TotalPresencasFinal { get; set; }
        public int TotalRemotoFinal { get; set; }
        public int TotalAusenciasFinal { get; set; }
        public int TotalCompensacoesFinal { get; set; }
        public double PercentualFrequenciaFinal { get; set; }
        public string TituloFinal { get; set; }
        public List<RelatorioFrequenciaIndividualBimestresDto> Bimestres { get; set; }
        public string DescricaoUltimoBimestre { get; set; }
    }
}
