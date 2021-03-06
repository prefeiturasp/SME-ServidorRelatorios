﻿using Newtonsoft.Json;

namespace SME.SR.Infra.Dtos.Requisicao
{
    public class ExecucaoRelatorioRequisicaoDto
    {
        [JsonProperty("reportUnitUri")]
        public string UnidadeRelatorioUri { get; set; }

        [JsonProperty("async")]
        public bool Async { get; set; }

        [JsonProperty("freshData")]
        public bool IgnorarCache { get; set; }

        [JsonProperty("saveDataSnapshot")]
        public bool SalvarSnapshot { get; set; }

        [JsonProperty("outputFormat")]
        public string FormatoSaida { get; set; }

        [JsonProperty("interactive")]
        public bool Interativo { get; set; }

        [JsonProperty("ignorePagination")]
        public bool? IgnorarPaginacao { get; set; }

        [JsonProperty("pages")]
        public bool? Paginas { get; set; }

        [JsonProperty("parameters")]
        public ParametrosRelatorioDto Parametros { get; set; }
    }
}
