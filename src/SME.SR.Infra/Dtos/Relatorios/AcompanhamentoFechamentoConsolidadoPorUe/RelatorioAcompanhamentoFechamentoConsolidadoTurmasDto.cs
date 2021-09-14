using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioAcompanhamentoFechamentoConsolidadoTurmasDto
    {
        public RelatorioAcompanhamentoFechamentoConsolidadoTurmasDto(string turmaDescricao)
        {
            TurmaDescricao = turmaDescricao;
            Bimestres = new List<RelatorioAcompanhamentoFechamentoConsolidadoBimestresDto>();
        }

        public string TurmaDescricao { get; set; }
        public List<RelatorioAcompanhamentoFechamentoConsolidadoBimestresDto> Bimestres { get; set; }

    }
}
