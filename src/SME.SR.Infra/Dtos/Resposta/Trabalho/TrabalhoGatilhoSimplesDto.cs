using Newtonsoft.Json;
using Refit;
using System;

namespace SME.SR.Infra.Dtos
{
    public class TrabalhoGatilhoSimplesDto
    {
        [JsonProperty("timezone"), AliasAs("timezone")]
        public string TimeZone { get; set; }

        [JsonProperty("id"), AliasAs("id")]
        public int? Id { get; set; }

        [JsonProperty("version"), AliasAs("version")]
        public int? Versao { get; set; }
        
        [JsonProperty("calendarName"), AliasAs("calendarName")]
        public object NomeCalendario { get; set; }

        [JsonProperty("startType"), AliasAs("startType")]
        public int? TipoInicio { get; set; }

        [JsonProperty("startDate"), AliasAs("startDate")]
        [JsonConverter(typeof(DateFormatConverter), "yyyy-MM-dd HH:mm")]
        public DateTime? DataInicio { get; set; }

        [JsonProperty("endDate"), AliasAs("endDate")]
        [JsonConverter(typeof(DateFormatConverter), "yyyy-MM-dd HH:mm")]
        public DateTime? DataFim { get; set; }

        [JsonProperty("misfireInstruction"), AliasAs("misfireInstruction")]
        public int? InstrucaoMisFire { get; set; }

        [JsonProperty("occurrenceCount"), AliasAs("occurrenceCount")]
        public int? ContagemOcorrencias { get; set; }

        [JsonProperty("recurrenceInterval"), AliasAs("recurrenceInterval")]
        public object IntervaloRecorrencia { get; set; }
    }
}
