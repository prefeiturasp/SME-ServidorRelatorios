using System;
using System.Globalization;

namespace SME.SR.Infra.Dtos
{
    public class FrequenciaAlunoConsolidadoDto
    {
        public int Bimestre { get; set; }
        public int TotalAula { get; set; }
        public int TotalPresencas { get; set; }
        public int TotalRemotos { get; set; }
        public int TotalAusencias { get; set; }
        public int TotalCompensacoes { get; set; }
        public string CodigoAluno { get; set; }
        public string AnoBimestre { get; set; }
        public int NumeroFaltasNaoCompensadas { get => TotalAusencias - TotalCompensacoes; }        
        public string BimestreFormatado
        {
            get
            {
                return $"{Bimestre}° BIMESTRE - {AnoBimestre}";
            }
        }
    }
}
