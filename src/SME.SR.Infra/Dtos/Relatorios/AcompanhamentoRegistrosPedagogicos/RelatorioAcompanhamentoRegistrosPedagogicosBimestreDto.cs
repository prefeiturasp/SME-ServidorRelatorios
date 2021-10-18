using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class RelatorioAcompanhamentoRegistrosPedagogicosBimestreDto
    {
        public RelatorioAcompanhamentoRegistrosPedagogicosBimestreDto()
        {

        }
        public RelatorioAcompanhamentoRegistrosPedagogicosBimestreDto(string bimestre, List<RelatorioAcompanhamentoRegistrosPedagogicosTurmaDto> turmas)
        {
            Bimestre = bimestre;
            Turmas = turmas;
        }
        public string Bimestre { get; set; }
        public List<RelatorioAcompanhamentoRegistrosPedagogicosTurmaDto> Turmas { get; set; }
    }
}
