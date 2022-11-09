using System.ComponentModel.DataAnnotations;

namespace SME.SR.Infra
{
    public class FrequenciaGlobalDto
    {
        [Display(Description = "DRE Cod.")]
        public string DreCodigo { get; set; }

        [Display(Description = "DRE")]
        public string SiglaDre { get; set; }

        [Display(Description = "UE Cod.")]
        public string UeCodigo { get; set; }

        [Display(Description = "UE")]
        public string UeNome { get; set; }

        [Display(Description = "Mês")]
        public int Mes { get; set; }

        [Display(Description = "Turma Cod.")]
        public string TurmaCodigo { get; set; }

        [Display(Description = "Turma")]
        public string Turma {  get; set; }

        [Display(Description = "Código EOL")]
        public string CodigoEOL { get; set; }

        [Display(Description = "Número de Chamada")]
        public string NumeroChamadda { get; set; }

        [Display(Description = "Estudante")]
        public string Estudante { get; set; }

        [Display(Description = "% Frequência")]
        public decimal? PercentualFrequencia { get; set; }
    }
}
