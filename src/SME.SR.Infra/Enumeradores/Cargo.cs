using System.ComponentModel.DataAnnotations;

namespace SME.SR.Infra
{
    public enum Cargo
    {
        [Display(Name = "Coordenador Pedagógico")]
        CP = 3379,

        [Display(Name = "Assistente Diretor")]
        AD = 3085,

        [Display(Name = "Diretor de Escola")]
        Diretor = 3360,

        [Display(Name = "Secretário")]
        Secretario = 3182,

        [Display(Name = "Supervisor Escolar")]
        Supervisor = 3352,

        [Display(Name = "Supervisor Técnico")]
        SupervisorTecnico = 434


    }
}
