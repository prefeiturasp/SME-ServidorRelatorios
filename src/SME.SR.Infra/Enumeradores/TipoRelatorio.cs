using System.ComponentModel.DataAnnotations;

namespace SME.SR.Infra
{
    public enum TipoRelatorio
    {
        [Display(Name = "relatorios/conselhoclasseatafinal", ShortName = "RelatorioConselhoClasseAtaFinal", Description = "Conselho Classe Ata Final")]
        ConselhoClasseAtaFinal = 5,
    }
}
