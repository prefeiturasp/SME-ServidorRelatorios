using Newtonsoft.Json;
using System;

namespace SME.SR.Data
{
    public class FechamentoAlunoAnotacaoConselho
    {
        [JsonProperty("anotacao")]
        public string Anotacao { get; set; }

        [JsonProperty("data")]
        public string Data { get; set; }

        [JsonProperty("disciplina")]
        public string Disciplina { get; set; }

        [JsonProperty("disciplinaId")]
        public string DisciplinaId { get; set; }

        [JsonProperty("professor")]
        public string Professor { get; set; }

        [JsonProperty("professorRf")]
        public string ProfessorRf { get; set; }
    }
}
