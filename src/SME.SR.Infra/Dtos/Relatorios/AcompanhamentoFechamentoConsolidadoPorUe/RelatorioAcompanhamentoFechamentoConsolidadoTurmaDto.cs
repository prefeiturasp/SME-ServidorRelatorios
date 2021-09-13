using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class RelatorioAcompanhamentoFechamentoConsolidadoTurmaDto
    {
        public RelatorioAcompanhamentoFechamentoConsolidadoTurmaDto(string nomeTurma, string nomeUe, string nomeBimestre)
        {
            FechamentoConsolidadoBimestre = new List<RelatorioAcompanhamentoFechamentoConsolidadoBimestreDto>();
            NomeTurma = nomeTurma;
            NomeUe = nomeUe;
            NomeBimestre = nomeBimestre;
        }
        public string NomeTurma { get; set; }
        public string NomeUe { get; set; }
        public string NomeBimestre { get; set; }
        public List<RelatorioAcompanhamentoFechamentoConsolidadoBimestreDto> FechamentoConsolidadoBimestre { get; set; }
    }
}
