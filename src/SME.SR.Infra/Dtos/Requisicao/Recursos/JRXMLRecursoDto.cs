using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra.Dtos.Requisicao.Recursos
{
   public class JRXMLRecursoDto
    {
        [JsonProperty("jrxmlFileReference")]
        public CaminhoArquivoRecursoDto CaminhoArquivo { get; set; }

        [JsonProperty("jrxmlFile")]
        public ArquivoJRXMLRecursoDto Arquivo{ get; set; }
    }
}
