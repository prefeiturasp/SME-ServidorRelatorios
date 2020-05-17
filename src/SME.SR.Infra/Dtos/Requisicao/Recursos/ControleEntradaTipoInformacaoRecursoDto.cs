using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra.Dtos.Requisicao.Recursos
{
   public class ControleEntradaTipoInformacaoRecursoDto
    {
        [JsonProperty("dataTypeReference")]
        public CaminhoArquivoRecursoDto CaminhoArquivo { get; set; }
    }
}
