﻿using Newtonsoft.Json;

namespace SME.SR.Infra.Dtos
{
    public class ControleEntradaRecursoDto : DetalhesRecursoDto
    {
        [JsonProperty("mandatory")]
        public bool Obrigatorio { get; set; }

        [JsonProperty("readOnly")]
        public bool SomenteLeitura { get; set; }

        [JsonProperty("visible")]
        public string Visivel { get; set; }

        [JsonProperty("type")]
        public string Tipo { get; set; }

        [JsonProperty("usedFields")]
        public string CamposUtilizados { get; set; }

        [JsonProperty("dataType")]
        public ControleEntradaTipoInformacaoRecursoDto TipoInformacao { get; set; }

        [JsonProperty("listOfValues")]
        public ControleEntradaListaValoresRecursoDto ListaValores { get; set; }

        [JsonProperty("visibleColumns")]
        public string[] ColunasVisiveis { get; set; }

        [JsonProperty("valueColumn")]
        public string ColunaValor { get; set; }

        [JsonProperty("query")]
        public QueryReferenciaRecursoDto Query { get; set; }
    }
}
