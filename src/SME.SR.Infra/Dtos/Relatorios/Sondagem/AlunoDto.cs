using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra.Dtos.Relatorios.Sondagem
{
    public class AlunoDto
    {
        [JsonProperty("codigo")]
        public long Codigo { get; set; }

        [JsonProperty("nome")]
        public string Nome { get; set; }

        [JsonProperty("situacaoMatricula")]
        public SituacaoMatriculaAluno SituacaoMatricula { get; set; }
    }
}
