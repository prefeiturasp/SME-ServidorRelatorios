using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class RelatorioSondagemComponentesPorTurmaPlanilhaLinhasDto
    {
        public RelatorioSondagemComponentesPorTurmaAlunoDto Aluno { get; set; }
        public List<RelatorioSondagemComponentesPorTurmaOrdemRespostasDto> OrdensRespostas { get; set; } = new List<RelatorioSondagemComponentesPorTurmaOrdemRespostasDto>();
        public RelatorioSondagemComponentesPorTurmaPlanilhaLinhasDto()
        {
            this.OrdensRespostas = new List<RelatorioSondagemComponentesPorTurmaOrdemRespostasDto>();
        }

    }
}