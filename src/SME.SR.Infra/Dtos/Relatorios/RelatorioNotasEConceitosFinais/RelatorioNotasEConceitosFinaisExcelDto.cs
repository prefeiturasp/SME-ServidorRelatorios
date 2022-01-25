using System.ComponentModel.DataAnnotations;

namespace SME.SR.Infra
{
   public class RelatorioNotasEConceitosFinaisExcelDto
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

        [Display(Description = "Nota/Conceito")]
        public string NotaConceito { get; set; }
        public bool EmAprovacao { get; set; }
    }
}
