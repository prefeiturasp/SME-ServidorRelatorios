using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioAcompanhamentoFechamentoConsolidadoPorUeDto
    {
        public RelatorioAcompanhamentoFechamentoConsolidadoPorUeDto()
        {
            Ues =  new List<RelatorioAcompanhamentoFechamentoConsolidadoUesDto>();
        }
        public string DreNome { get; set; }
        public string UeNome { get; set; }
        public string Bimestre { get; set; }
        public string Turma { get; set; }
        public string Usuario { get; set; }
        public string RF { get; set; }
        public string Data { get; set; }
        public List<RelatorioAcompanhamentoFechamentoConsolidadoUesDto> Ues { get; set; }
    }
}
