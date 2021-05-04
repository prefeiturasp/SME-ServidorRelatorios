using System.ComponentModel.DataAnnotations;

namespace SME.SR.Infra
{
    public enum AtaFinalTipoVisualizacao
    {
        [Display(Name = "Turma")]
        Turma = 1,

        [Display(Name = "Estudantes")]
        Estudantes = 2,

    }
}
