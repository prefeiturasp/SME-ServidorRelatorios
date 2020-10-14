using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class RelatorioSondagemPortuguesPorTurmaPlanilhaLinhasDto
    {
        public RelatorioSondagemComponentesPorTurmaAlunoDto Aluno { get; set; }
        public List<RelatorioSondagemPortuguesPorTurmaRespostasDto> Respostas { get; set; } = new List<RelatorioSondagemPortuguesPorTurmaRespostasDto>();
        public RelatorioSondagemPortuguesPorTurmaPlanilhaLinhasDto()
        {
            this.Respostas = new List<RelatorioSondagemPortuguesPorTurmaRespostasDto>();
        }
    }
}
