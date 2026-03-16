using System.ComponentModel.DataAnnotations;

namespace SME.SR.Infra
{
    public enum TipoRelatorio
    {
        [Display(Name = "relatorios/atafinalresultados", ShortName = "RelatorioAtaFinalResultados", Description = "Ata final de resultados")]
        ConselhoClasseAtaFinal = 5,

        [Display(Name = "relatorios/frequencia-global-todos", ShortName = "RelatorioFrequenciaMensalTodosDreUe", Description = "Relatório de frequência mensal filtro todos dre ou ue")]
        FrequenciaMensalTodosDreUe = 57,
    }
}
