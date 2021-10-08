using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class RelatorioAcompanhamentoRegistrosPedagogicosBimestreDto
    {
        public int Bimestre { get; set; }
        public List<RelatorioAcompanhamentoRegistrosPedagogicosTurmaDto> Turmas { get; set; }

    }
}
