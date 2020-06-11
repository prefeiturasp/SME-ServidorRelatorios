using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace SME.SR.Data
{
    public abstract class RelatorioConselhoClasseBase
    {
        [JsonProperty("titulo")]
        public string Titulo { get; set; }

        [JsonProperty("dre")]
        public string Dre { get; set; }

        [JsonProperty("ue")]
        public string Ue { get; set; }

        [JsonProperty("turma")]
        public string Turma { get; set; }

        [JsonProperty("data")]
        public string Data { get; set; }

        [JsonProperty("alunoNome")]
        public string AlunoNome { get; set; }

        [JsonProperty("alunoNumero")]
        public int AlunoNumero { get; set; }

        [JsonProperty("alunoDataDeNascimento")]
        public string AlunoDataDeNascimento { get; set; }

        [JsonProperty("alunoCodigoEol")]
        public string AlunoCodigoEol { get; set; }

        [JsonProperty("alunoSituacao")]
        public string AlunoSituacao { get; set; }

        [JsonProperty("alunoFrequenciaGlobal")]
        public double AlunoFrequenciaGlobal { get; set; }

        [JsonProperty("alunoParecerConclusivo")]
        public string AlunoParecerConclusivo { get; set; }

        [JsonProperty("bimestre")]
        public string Bimestre { get; set; }

        [JsonProperty("anotacoesPedagogicas")]
        public string AnotacoesPedagogicas { get; set; }

        [JsonProperty("recomendacaoAluno")]
        public string RecomendacaoAluno { get; set; }

        [JsonProperty("recomendacaoFamilia")]
        public string RecomendacaoFamilia { get; set; }

        [JsonProperty("anotacoesAluno")]
        public IEnumerable<FechamentoAlunoAnotacaoConselho> AnotacoesAluno { get; set; }
    }
}
