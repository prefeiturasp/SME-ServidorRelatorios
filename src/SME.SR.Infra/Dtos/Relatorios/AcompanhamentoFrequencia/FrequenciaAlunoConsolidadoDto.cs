using System;
using System.Globalization;

namespace SME.SR.Infra.Dtos
{
    public class FrequenciaAlunoConsolidadoDto
    {
        public const int PERCENTUAL_FREQUENCIA_PRECISAO = 2;
        public int Bimestre { get; set; }
        public int TotalAula { get; set; }
        public int TotalPresencas { get; set; }
        public int TotalRemotos { get; set; }
        public int TotalAusencias { get; set; }
        public int TotalCompensacoes { get; set; }
        public string CodigoAluno { get; set; }
        public string AnoBimestre { get; set; }
        public int NumeroFaltasNaoCompensadas { get => TotalAusencias - TotalCompensacoes; }
        public double TotalPercentualFrequencia
        {
            get
            {
                if (TotalAula == 0)
                    return 0;

                var porcentagem = 100 - ((double)NumeroFaltasNaoCompensadas / TotalAula) * 100;
                return ArredondarPercentual(porcentagem > 100 ? 100 : porcentagem);
            }
        }        
        public static double ArredondarPercentual(double percentual) => Math.Round(percentual, PERCENTUAL_FREQUENCIA_PRECISAO);
        public string BimestreFormatado
        {
            get
            {
                return $"{Bimestre}° BIMESTRE - {AnoBimestre}";
            }
        }
        public string TotalPercentualFrequenciaFormatado
        {
            get
            {
                return TotalAula == 0 ? "" : $"{TotalPercentualFrequencia.ToString($"N{PERCENTUAL_FREQUENCIA_PRECISAO}", CultureInfo.CurrentCulture)}%";
            }
        }
    }
}
