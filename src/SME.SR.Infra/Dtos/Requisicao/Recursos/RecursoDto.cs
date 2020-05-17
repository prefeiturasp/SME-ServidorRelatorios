using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra.Dtos.Requisicao.Recursos
{
   public class RecursoDto
    {
        [JsonProperty("name")]
        public CaminhoArquivoRecursoDto CaminhoArquivo { get; set; }

        [JsonProperty("file")]
        public string Conteudo { get; set; }

        [JsonProperty("fileReference")]
        public CaminhoArquivoRecursoDto ReferenciaArquivo { get; set; }
    }
}
