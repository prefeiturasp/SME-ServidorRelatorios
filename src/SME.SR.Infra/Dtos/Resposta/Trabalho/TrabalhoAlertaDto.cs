using Newtonsoft.Json;
using Refit;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra.Dtos
{
    public class TrabalhoAlertaDto
    {
        [JsonProperty("id"), AliasAs("id")]
        public int? Id { get; set; }

        [JsonProperty("version"), AliasAs("version")]
        public int? Versao { get; set; }

        [JsonProperty("recipient"), AliasAs("recipient")]
        public string Recipiente { get; set; }

        [JsonProperty("toAddresses"), AliasAs("toAddresses")]
        public TrabalhoAlertaEnderecoDto Enderecos { get; set; }

        [JsonProperty("jobState"), AliasAs("jobState")]
        public string EstadoTrabalho { get; set; }

        [JsonProperty("messageText"), AliasAs("messageText")]
        public string TextoMensagem { get; set; }

        [JsonProperty("messageTextWhenJobFails"), AliasAs("messageTextWhenJobFails")]
        public string TextoMensagemFalha { get; set; }

        [JsonProperty("subject"), AliasAs("subject")]
        public string Assunto { get; set; }

        [JsonProperty("includingStackTrace"), AliasAs("includingStackTrace")]
        public bool? StackTraceIncluso { get; set; }

        [JsonProperty("includingReportJobInfo"), AliasAs("includingReportJobInfo")]
        public bool? InformacaoTrabalhoRelatorioIncluso { get; set; }
    }
}
