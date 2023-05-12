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
        public string FrequenciaDoPeriodo { get; set; }
        public List<ControleFrequenciaPorTipoDto> FrequenciaPorTipo { get; set; }
    }
}