using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra.Dtos
{
   public class PadraoConcessaoAcessoRecursoDto
    {
        [JsonProperty("accessGrantSchemaReference")]
        public CaminhoArquivoRecursoDto CaminhoArquivo { get; set; }
    }
}
