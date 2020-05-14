using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra.Dtos.Resposta.ControlesEntrada
{
    public class ControleEntradaDto
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("type")]
        public string Tipo { get; set; }

        [JsonProperty("description")]
        public string Descricao { get; set; }

        [JsonProperty("uri")]
        public string Uri { get; set; }

        [JsonProperty("label")]
        public string Label { get; set; }

        [JsonProperty("mandatory")]
        public bool Obrigatorio { get; set; }

        [JsonProperty("readOnly")]
        public bool ApenasLeitura { get; set; }

        [JsonProperty("visible")]
        public bool Visivel { get; set; }

        [JsonProperty("masterDependencies")]
        public IList<string> MasterDependencies { get; set; }

        [JsonProperty("slaveDependencies")]
        public IList<string> SlaveDependencies { get; set; }

        [JsonProperty("state")]
        public EstadoDto Estado { get; set; }

        [JsonProperty("dataType")]
        public TipoDadosDto TipoDados { get; set; }

        [JsonProperty("validationRules")]
        public IList<RegraValidacaoDto> RegraValidacao { get; set; }
    }
}
