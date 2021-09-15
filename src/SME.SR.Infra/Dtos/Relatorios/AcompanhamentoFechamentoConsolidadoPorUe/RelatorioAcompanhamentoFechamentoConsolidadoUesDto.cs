using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioAcompanhamentoFechamentoConsolidadoUesDto
    {
        public RelatorioAcompanhamentoFechamentoConsolidadoUesDto(string nomeUe)
        {
            NomeUe = nomeUe;
            Bimestres = new List<RelatorioAcompanhamentoFechamentoConsolidadoBimestresDto>();
        }

        public string NomeUe { get; set; }
        public List<RelatorioAcompanhamentoFechamentoConsolidadoBimestresDto> Bimestres { get; set; }
    }
}
