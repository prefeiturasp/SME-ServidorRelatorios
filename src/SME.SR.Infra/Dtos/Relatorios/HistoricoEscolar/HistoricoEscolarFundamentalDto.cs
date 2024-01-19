using Newtonsoft.Json;
using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class HistoricoEscolarFundamentalDto : HistoricoEscolarDto
    {
        
        [JsonProperty("historicoEscolar")]
        public HistoricoEscolarNotasFrequenciaDto DadosHistorico { get; set; }
        public bool EhMagisterio { get; set; }
        public override bool ContemDadosHistorico()
        {
            return DadosHistorico != null;
        }

    }
}
