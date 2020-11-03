using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaPlanilhaLinhasDto
    {
        public RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaAlunoDto Aluno { get; set; }
        public List<RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaOrdemRespostasDto> OrdensRespostas { get; set; } = new List<RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaOrdemRespostasDto>();
        public RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaPlanilhaLinhasDto()
        {
            this.OrdensRespostas = new List<RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaOrdemRespostasDto>();
        }

    }
}