using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra.Dtos.Resposta
{
    public class ExportacaoDetalhesDto
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("options")]
        public OpcaoDto Opcoes { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("outputResource")]
        public SaidaRecursoDto SaidaRecurso { get; set; }

        [JsonProperty("attachments")]
        public AnexoDto[] Anexos { get; set; }
    }
}
