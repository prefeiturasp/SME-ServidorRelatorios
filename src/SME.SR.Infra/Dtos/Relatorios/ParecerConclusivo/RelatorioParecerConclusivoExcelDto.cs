using System.ComponentModel.DataAnnotations;

namespace SME.SR.Infra
{
    public class RelatorioParecerConclusivoExcelDto
    {
        [Display(Description = "Nome DRE")]
        public string  NomeDre { get; set; }

        [Display(Description = "Nome da UE")]
        public string NomeUe { get; set; }

        public string Ciclo { get; set; }

        public string Ano { get; set; }

        public string Turma { get; set; }

        [Display(Description = "Código EOL")]
        public string CodigoAluno { get; set; }

        [Display(Description = "Nome do Estudante")]
        public string NomeAluno { get; set; }

        [Display(Description = "Parecer Conclusivo")]
        public string ParecerConclusivo { get; set; }
    }
}
