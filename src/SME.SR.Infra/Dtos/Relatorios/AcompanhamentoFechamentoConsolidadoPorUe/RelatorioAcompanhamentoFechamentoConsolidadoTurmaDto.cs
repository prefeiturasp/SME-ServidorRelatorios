using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class RelatorioAcompanhamentoFechamentoConsolidadoTurmaDto
    {
        public RelatorioAcompanhamentoFechamentoConsolidadoTurmaDto(string turmaDescricao)
        {
            TurmaDescricao = turmaDescricao;
            FechamentoConsolidadoBimestre = new List<RelatorioAcompanhamentoFechamentoConsolidadoBimestreDto>();
        }
        public string TurmaDescricao { get; set; }
        public List<RelatorioAcompanhamentoFechamentoConsolidadoBimestreDto> FechamentoConsolidadoBimestre { get; set; }
    }
}
