using System.ComponentModel.DataAnnotations;

namespace SME.SR.Infra
{
    public enum TipoRelatorio
    {
        [Display(Name = "relatorios/conselhoclasseatafinal", ShortName = "RelatorioConselhoClasseAtaFinal", Description = "Ata final de resultados")]
        ConselhoClasseAtaFinal = 5,
    }
}
