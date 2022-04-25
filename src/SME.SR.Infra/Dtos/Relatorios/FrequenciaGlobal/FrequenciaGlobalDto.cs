using System.ComponentModel.DataAnnotations;

namespace SME.SR.Infra
{
    public class FrequenciaGlobalDto
    {
        [Display(Description = "DRE")]
        public string CodigoDre { get; set; }
        [Display(Description = "UE")]
        public string CodigoUe { get; set; }
        [Display(Description = "Mês")]
        public int Mes { get; set; }
        [Display(Description = "Turma")]
        public string Turma {  get; set; }
        [Display(Description = "Código EOL")]
        public string CodigoEOL { get; set; }
        [Display(Description = "Estudante")]
        public string Estudante { get; set; }
        [Display(Description = "% Frequência")]
        public int PercentualFrequencia { get; set; }
    }
}
