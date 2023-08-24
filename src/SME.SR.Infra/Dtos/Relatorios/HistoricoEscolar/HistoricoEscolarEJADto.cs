using Newtonsoft.Json;
using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class HistoricoEscolarEJADto : HistoricoEscolarDto
    {
        [JsonProperty("historicoEscolar")]
        public HistoricoEscolarEJANotasFrequenciaDto DadosHistorico { get; set; }
        public override bool ContemDadosHistorico()
        {
            return DadosHistorico != null;
        }
    }
}
