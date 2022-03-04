using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioAcompanhamentoRegistrosPedagogicosInfantilComponenteDto
    {
        public RelatorioAcompanhamentoRegistrosPedagogicosInfantilComponenteDto()
        {
            Bimestres = new List<RelatorioAcompanhamentoRegistrosPedagogicosBimestreInfantilComponenteDto>();
        }

        public RelatorioAcompanhamentoRegistrosPedagogicosInfantilComponenteDto(RelatorioAcompanhamentoRegistrosPedagogicosCabecalhoDto cabecalho) : this()
        {
            Cabecalho = cabecalho;
        }

        public RelatorioAcompanhamentoRegistrosPedagogicosCabecalhoDto Cabecalho { get; set; }
        public List<RelatorioAcompanhamentoRegistrosPedagogicosBimestreInfantilComponenteDto> Bimestres { get; set; }

    }
}
