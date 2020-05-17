using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra.Dtos.Requisicao.Recursos
{
    public class ControleEntradaListaValoresRecursoDto
    {
        [JsonProperty("listOfValuesReference")]
        public CaminhoArquivoRecursoDto CaminhoArquivo { get; set; }
    }
}
