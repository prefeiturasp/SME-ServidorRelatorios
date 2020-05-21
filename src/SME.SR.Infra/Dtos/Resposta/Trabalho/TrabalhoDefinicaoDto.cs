using Newtonsoft.Json;
using Refit;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra.Dtos
{
    public class TrabalhoDefinicaoDto
    {
        [JsonProperty("id"),AliasAs("id")]
        public int? Id { get; set; }

        [JsonProperty("version"), AliasAs("version")]
        public int? Versao { get; set; }

        [JsonProperty("username"), AliasAs("username")]
        public string? Usuario { get; set; }

        [JsonProperty("label"), AliasAs("label")]
        public string? Texto { get; set; }

        [JsonProperty("description"), AliasAs("description")]
        public string? Descricao { get; set; }

        [JsonProperty("creationDate"), AliasAs("creationDate")]
        public DateTime? DataCriacao { get; set; }

        [JsonProperty("outputFormats"), AliasAs("outputFormats")]
        public TrabalhoFormatoSaidaDto? FormatosSaidas { get; set; }

        [JsonProperty("trigger"), AliasAs("trigger")]
        public TrabalhoGatilhoDto? Gatilhos { get; set; }

        [JsonProperty("source"), AliasAs("source")]
        public TrabalhoOrigemDto? Origem { get; set; }

        [JsonProperty("alert"), AliasAs("alert")]
        public TrabalhoAlertaDto? Alerta { get; set; }

        [JsonProperty("baseOutputFilename"), AliasAs("baseOutputFilename")]
        public string? BaseNomeArquivoSaida { get; set; }

        [JsonProperty("outputLocale"), AliasAs("outputLocale")]
        public string? LocalSaida { get; set; }

        [JsonProperty("mailNotification"), AliasAs("mailNotification")]
        public string? NotificacaoMail { get; set; }

        [JsonProperty("outputTimeZone"), AliasAs("outputTimeZone")]
        public string? TimeZoneSaida { get; set; }

        [JsonProperty("repositoryDestination"), AliasAs("repositoryDestination")]
        public TrabalhoRepositorioDestinoDto? RepositorioDestino { get; set; }
    }
}
