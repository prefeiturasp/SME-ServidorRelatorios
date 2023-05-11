using System.Collections.Generic;

namespace SME.SR.Infra.Dtos.FrequenciaMensal
{
    public class ControleFrequenciaPorComponenteDto
    {
        public ControleFrequenciaPorComponenteDto()
        {
            FrequenciaPorAula = new List<ControleFrequenciaPorAulaDto>();
        }
        public string NomeComponente { get; set; }
        public string FrequenciaPeriodo { get; set; }
        public List<ControleFrequenciaPorAulaDto> FrequenciaPorAula { get; set; }
        public int TotalPeriodoAulas { get; set; }
        public int TotalPeriodoPresenca { get; set; }
        public int TotalPeriodoCompesacao { get; set; }
    }
}