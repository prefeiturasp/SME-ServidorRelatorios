using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class RelatorioAcompanhamentoRegistrosPedagogicosTurmaDto
    {
        public RelatorioAcompanhamentoRegistrosPedagogicosTurmaDto()
        {

        }
        public RelatorioAcompanhamentoRegistrosPedagogicosTurmaDto(string nome)
        {
            Nome = nome;
            ComponentesCurriculares = new List<RelatorioAcompanhamentoRegistrosPedagogicosCompCurricularesDto>();
        }
        public string Nome { get; set; }
        public List<RelatorioAcompanhamentoRegistrosPedagogicosCompCurricularesDto> ComponentesCurriculares { get; set; }
    }
}
