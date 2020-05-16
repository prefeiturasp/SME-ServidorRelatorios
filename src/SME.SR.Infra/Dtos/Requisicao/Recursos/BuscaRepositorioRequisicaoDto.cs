using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra.Dtos
{
    public class BuscaRepositorioRequisicaoDto
    {
        [JsonProperty("q")]
        public string Query { get; set; }

        [JsonProperty("folderUri")]
        public string CaminhoPasta { get; set; }

        [JsonProperty("recursive")]
        public bool Recursivo { get; set; }

        [JsonProperty("type")]
        public string Tipo { get; set; }

        [JsonProperty("accessType")]
        public long TipoAcesso { get; set; }

        [JsonProperty("dependsOn")]
        public string DiretorioDependencia { get; set; }

        [JsonProperty("showHiddenItems")]
        public bool ExibirItensOcultos { get; set; }

        [JsonProperty("sortBy")]
        public string Ordenacao { get; set; }

        [JsonProperty("limit")]
        public int LimitePaginacao { get; set; }

        [JsonProperty("offset")]
        public int Offset { get; set; }

        [JsonProperty("forceFullPage")]
        public bool ForcarPaginaInteira { get; set; }

        [JsonProperty("forceTotalCount")]
        public bool ForcarContadorPaginas { get; set; }
    }
}
