using System.ComponentModel.DataAnnotations;

namespace SME.SR.Infra
{
    public class RelatorioFrequenciasExcelDto : RelatorioFaltasFrequenciasBaseExcelDto
    {
        [Display(Description = "Nome DRE")]
        public string DreNome { get; set; }

        [Display(Description = "Nome UE")]
        public string UnidadeEscolarNome { get; set; }

        public string Bimestre { get; set; }

        public string Ano { get; set; }

        public string Turma { get; set; }

        [Display(Description = "Componente Curricular")]
        public string ComponenteCurricular { get; set; }

        [Display(Description = "Código EOL")]
        public string EstudanteCodigo { get; set; }

        [Display(Description = "Nome Estudante")]
        public string EstudanteNome { get; set; }

        [Display(Description = "% Frequência")]
        public double FrequenciaPercentual { get; set; }
    }
}
