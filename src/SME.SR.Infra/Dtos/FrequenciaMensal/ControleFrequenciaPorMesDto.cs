using System.Collections.Generic;

namespace SME.SR.Infra.Dtos.FrequenciaMensal
{
    public class ControleFrequenciaPorMesDto
    {
        public ControleFrequenciaPorMesDto()
        {
            FrequenciaComponente = new List<ControleFrequenciaPorComponenteDto>();
        }
        public string MesDescricao { get; set; }
        public int Mes { get; set; }
        public string FrequenciaGlobal { get; set; }
        public List<ControleFrequenciaPorComponenteDto> FrequenciaComponente { get; set; }
    }
}