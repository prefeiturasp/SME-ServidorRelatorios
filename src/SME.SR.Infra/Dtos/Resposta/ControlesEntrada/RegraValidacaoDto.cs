using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra.Dtos.Resposta.ControlesEntrada
{
    public class RegraValidacaoDto
    {
        [JsonProperty("dateTimeFormatValidationRule")]
        public RegraValidacaoFormatoDataHoraDto RegraValidacaoFormatoDataHora { get; set; }

        [JsonProperty("mandatoryValidationRule")]
        public RegraValidacaoObrigatoriaDto RegraValidacaoObrigatoria { get; set; }
    }
}
