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
    }
}
