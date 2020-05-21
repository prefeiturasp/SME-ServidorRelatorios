using Newtonsoft.Json;
using Refit;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra.Dtos
{
    public class TrabalhoInformacaoFTPSaidaDto
    {
        [JsonProperty("userName"), AliasAs("userName")]
        public string Usuario { get; set; }

        [JsonProperty("password"), AliasAs("password")]
        public string Senha { get; set; }

        [JsonProperty("folderPath"), AliasAs("folderPath")]
        public string CaminhoSaida { get; set; }

        [JsonProperty("serverName"), AliasAs("serverName")]
        public string NomeServidor { get; set; }

        [JsonProperty("type"), AliasAs("type")]
        public string Tipo { get; set; }

        [JsonProperty("protocol"), AliasAs("protocol")]
        public string Protocolo { get; set; }

        [JsonProperty("port"), AliasAs("port")]
        public int Porta { get; set; }

        [JsonProperty("implicit"), AliasAs("implicit")]
        public bool Implicito { get; set; }

        [JsonProperty("pbsz"), AliasAs("pbsz")]
        public int Pbsz { get; set; }

        [JsonProperty("prot"), AliasAs("prot")]
        public string Prot { get; set; }
    }
}
