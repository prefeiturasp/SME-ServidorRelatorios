﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace SME.SR.Data
{
    public abstract class RelatorioConselhoClasseBase
    {        
        [JsonProperty("Titulo")]
        public string Titulo { get; set; }

        [JsonProperty("Dre")]
        public string Dre { get; set; }

        [JsonProperty("Ue")]
        public string Ue { get; set; }

        [JsonProperty("Turma")]
        public string Turma { get; set; }

        [JsonProperty("Data")]
        public string Data { get; set; }

        [JsonProperty("AlunoNome")]
        public string AlunoNome { get; set; }

        [JsonProperty("AlunoNumero")]
        public int AlunoNumero { get; set; }

        [JsonProperty("AlunoDataDeNascimento")]
        public string AlunoDataDeNascimento { get; set; }

        [JsonProperty("AlunoCodigoEol")]
        public string AlunoCodigoEol { get; set; }

        [JsonProperty("AlunoSituacao")]
        public string AlunoSituacao { get; set; }

        [JsonProperty("AlunoFrequenciaGlobal")]
        public string AlunoFrequenciaGlobal { get; set; }

        [JsonProperty("AlunoParecerConclusivo")]
        public string AlunoParecerConclusivo { get; set; }

        [JsonProperty("Bimestre")]
        public string Bimestre { get; set; }

        [JsonProperty("AnotacoesPedagogicas")]
        public string AnotacoesPedagogicas { get; set; }

        [JsonProperty("RecomendacaoAluno")]
        public string RecomendacaoAluno { get; set; }

        [JsonProperty("RecomendacaoFamilia")]
        public string RecomendacaoFamilia { get; set; }

        [JsonProperty("AnotacoesAluno")]
        public IEnumerable<FechamentoAlunoAnotacaoConselho> AnotacoesAluno { get; set; }
        public bool EhBimestreFinal { get; set; }
    }
}
