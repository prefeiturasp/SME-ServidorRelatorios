using System.Collections.Generic;

namespace SME.SR.Infra.Dtos.FrequenciaMensal
{
    public class ControleFrequenciaPorComponenteDto
    {
        public ControleFrequenciaPorComponenteDto()
        {
            FrequenciaPorTipo = new List<ControleFrequenciaPorTipoDto>();
        }
        public string NomeComponente { get; set; }
        public string FrequenciaDoPeriodo { get; set; }
        public List<ControleFrequenciaPorTipoDto> FrequenciaPorTipo { get; set; }
    }
}