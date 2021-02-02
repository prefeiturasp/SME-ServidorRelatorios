using System.ComponentModel.DataAnnotations;

namespace SME.SR.Infra
{
    public enum TipoRelatorio
    {
        [Display(Name = "relatorios/atafinalresultados", ShortName = "RelatorioAtaFinalResultados", Description = "Ata final de resultados")]
        ConselhoClasseAtaFinal = 5,
    }
}
