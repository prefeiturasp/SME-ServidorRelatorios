using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class RelatorioSondagemComponentesPorTurmaPlanilhaDto
    {
        public RelatorioSondagemComponentesPorTurmaAlunoDto Aluno { get; set; }
        public List<RelatorioSondagemComponentesPorTurmaOrdemDto> Ordens { get; set; } = new List<RelatorioSondagemComponentesPorTurmaOrdemDto>();
        public List<RelatorioSondagemComponentesPorTurmaOrdemRespostasDto> OrdensRespostas { get; set; } = new List<RelatorioSondagemComponentesPorTurmaOrdemRespostasDto>();

        public RelatorioSondagemComponentesPorTurmaPlanilhaDto()
        {
            this.Ordens = new List<RelatorioSondagemComponentesPorTurmaOrdemDto>();
            this.OrdensRespostas = new List<RelatorioSondagemComponentesPorTurmaOrdemRespostasDto>();
        }
    }
}
