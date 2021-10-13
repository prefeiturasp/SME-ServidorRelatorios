using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class RelatorioAcompanhamentoRegistrosPedagogicosBimestreDto
    {
        public RelatorioAcompanhamentoRegistrosPedagogicosBimestreDto(string bimestre)
        {
            Bimestre = bimestre;
            Turmas = new List<RelatorioAcompanhamentoRegistrosPedagogicosTurmaDto>();
        }
        public string Bimestre { get; set; }
        public List<RelatorioAcompanhamentoRegistrosPedagogicosTurmaDto> Turmas { get; set; }
    }
}
