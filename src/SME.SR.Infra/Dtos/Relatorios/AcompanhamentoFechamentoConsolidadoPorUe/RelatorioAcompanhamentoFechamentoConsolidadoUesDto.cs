using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioAcompanhamentoFechamentoConsolidadoUesDto
    {
        public RelatorioAcompanhamentoFechamentoConsolidadoUesDto()
        {
            FechamentoConselhoClasseConsolidado = new List<RelatorioAcompanhamentoFechamentoConselhoClasseConsolidadoDto>();
        }

        public string NomeTurma { get; set; }
        public string NomeUe { get; set; }
        public string NomeBimestre { get; set; }
        //remover este comentario
        //remover este comentario
        //remover este comentario
        //remover este comentario
        //remover este comentario
        //remover este comentario
        public List<RelatorioAcompanhamentoFechamentoConselhoClasseConsolidadoDto> FechamentoConselhoClasseConsolidado { get; set; }
    }
}
