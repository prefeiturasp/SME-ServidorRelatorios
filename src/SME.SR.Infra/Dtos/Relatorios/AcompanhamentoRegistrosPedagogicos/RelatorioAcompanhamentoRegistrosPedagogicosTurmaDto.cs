using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class RelatorioAcompanhamentoRegistrosPedagogicosTurmaDto
    {
        public string Nome { get; set; }
        public List<RelatorioAcompanhamentoRegistrosPedagogicosCompCurricularesDto> ComponentesCurriculares { get; set; }
    }
}
