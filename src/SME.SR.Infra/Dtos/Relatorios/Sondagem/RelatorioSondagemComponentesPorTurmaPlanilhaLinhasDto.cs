using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioSondagemComponentesPorTurmaPlanilhaLinhasDto
    {
        public RelatorioSondagemComponentesPorTurmaAlunoDto Aluno { get; set; }
        public List<RelatorioSondagemComponentesPorTurmaOrdemRespostasDto> OrdensRespostas { get; set; } = new List<RelatorioSondagemComponentesPorTurmaOrdemRespostasDto>();

        public RelatorioSondagemComponentesPorTurmaPlanilhaLinhasDto()
        {
            OrdensRespostas = new List<RelatorioSondagemComponentesPorTurmaOrdemRespostasDto>();
        }
    }
}