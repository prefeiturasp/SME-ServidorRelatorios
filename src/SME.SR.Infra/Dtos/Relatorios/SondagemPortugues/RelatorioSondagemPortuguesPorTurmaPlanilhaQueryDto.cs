﻿using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class RelatorioSondagemPortuguesPorTurmaPlanilhaQueryDto
    {
        public string AlunoEolCode { get; set; }
        public string AlunoNome { get; set; }
        public int AnoLetivo { get; set; }
        public int AnoTurma { get; set; }
        public string TurmaEolCode { get; set; }
        public string PerguntaId { get; set; }
        public string Pergunta { get; set; }
        public string Resposta { get; set; }
    }
}
