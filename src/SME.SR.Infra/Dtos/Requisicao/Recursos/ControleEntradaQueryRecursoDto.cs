using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra.Dtos.Requisicao.Recursos
{
    public class ControleEntradaQueryRecursoDto
    {
        [JsonProperty("queryReference")]
        public CaminhoArquivoRecursoDto CaminhoArquivo { get; set; }
    }
}
