using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra.Dtos.Requisicao.ExecucaoRelatorio.PostModificarParametrosRelatorio
{
    public class ModificarParametrosRelatorioRequisicaoDto
    {
        [JsonProperty("name")]
        public ParametroDto[] Parametros { get; set; }
    }
}
