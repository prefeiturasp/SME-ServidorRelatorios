using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class RelatorioSondagemPortuguesPorTurmaPlanilhaLinhaDto
    {
        public RelatorioSondagemComponentesPorTurmaAlunoDto Aluno { get; set; }
        public List<RelatorioSondagemPortuguesPorTurmaRespostaDto> Respostas { get; set; } = new List<RelatorioSondagemPortuguesPorTurmaRespostaDto>();
        public RelatorioSondagemPortuguesPorTurmaPlanilhaLinhaDto()
        {
            this.Respostas = new List<RelatorioSondagemPortuguesPorTurmaRespostaDto>();
        }
    }
}
