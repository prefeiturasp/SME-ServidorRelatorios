using System.ComponentModel.DataAnnotations;

namespace SME.SR.Infra
{
    public class RelatorioFaltasFrequenciasExcelDto
    {
        [Display(Description = "Nome DRE")]
        public string DreNome { get; set; }
        [Display(Description = "Nome Escola")]
        public string UnidadeEscolarNome { get; set; }
        public int Ano { get; set; }
        public int Bimestre { get; set; }
        [Display(Description = "Código Estudante")]
        public string EstudanteCodigo { get; set; }
        public string Turma { get; set; }
        public string Disciplina { get; set; }
        [Display(Description = "Qtd Faltas")]
        public int FaltasQuantidade { get; set; }

        [Display(Description = "Qtd Aulas")]
        public int AulasQuantidade { get; set; }

        [Display(Description = "% Ausencia")]
        public double AusenciaPercentual { get; set; }
    }
}
