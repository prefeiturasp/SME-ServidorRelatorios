using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class RelatorioSondagemComponentesPorTurmaAlunoDto
    {
        public int Codigo { get; set; }
        public string Nome { get; set; }
        public string SituacaoMatricula { get; set; }
        public string DataSituacao { get; set; }
    }
}
