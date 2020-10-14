using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class RelatorioSondagemComponentesPorTurmaPlanilhaQueryDto
    {
        public string AlunoEolCode { get; set; }
        public string AlunoNome { get; set; }
        public int AnoLetivo { get; set; }
        public int AnoTurma { get; set; }
        public string TurmaEolCode { get; set; }
        public int Semestre { get; set; }
        public string Ordem1Ideia { get; set; }
        public string Ordem1Resultado { get; set; }
        public string Ordem2Ideia { get; set; }
        public string Ordem2Resultado { get; set; }
        public string Ordem3Ideia { get; set; }
        public string Ordem3Resultado { get; set; }
        public string Ordem4Ideia { get; set; }
        public string Ordem4Resultado { get; set; }
        public string Ordem5Ideia { get; set; }
        public string Ordem5Resultado { get; set; }
        public string Ordem6Ideia { get; set; }
        public string Ordem6Resultado { get; set; }
        public string Ordem7Ideia { get; set; }
        public string Ordem7Resultado { get; set; }
        public string Ordem8Ideia { get; set; }
        public string Ordem8Resultado { get; set; }
        public string Familiares { get; set;  }
        public string Opacos { get; set; }
        public string Transparentes { get; set; }
        public string TerminamZero { get; set; }
        public string Algarismos { get; set; }
        public string Processo { get; set; }
        public string ZeroIntercalados { get; set; }
        public int PerguntaId { get; set; }
        public string Pergunta { get; set; }
        public string Resposta { get; set; }

    }
}
