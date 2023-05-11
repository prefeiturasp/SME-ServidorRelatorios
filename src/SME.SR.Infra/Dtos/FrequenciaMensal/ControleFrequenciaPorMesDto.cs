using System.Collections.Generic;

namespace SME.SR.Infra.Dtos.FrequenciaMensal
{
    public class ControleFrequenciaPorMesDto
    {
        public string Mes { get; set; }
        public List<ControleFrequenciaPorComponenteDto> FrequenciaComponente { get; set; }
    }
}