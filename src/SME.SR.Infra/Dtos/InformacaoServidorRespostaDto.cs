using Newtonsoft.Json;

namespace SME.SR.Infra.Dtos
{
    public class InformacaoServidorRespostaDto
    {
        [JsonProperty("dateFormatPattern")]
        public string FormatoData { get; set; }

        [JsonProperty("datetimeFormatPattern")]
        public string FormatoDataHora { get; set; }

        [JsonProperty("version")]
        public string Versao { get; set; }

        [JsonProperty("edition")]
        public string Edicao { get; set; }

        [JsonProperty("editionName")]
        public string NomeEdicao { get; set; }

        [JsonProperty("licenseType")]
        public string TipoLicenca { get; set; }

        [JsonProperty("build")]
        public string Build { get; set; }

        [JsonProperty("features")]
        public string Features { get; set; }
    }
}
