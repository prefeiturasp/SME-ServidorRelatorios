using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioAcompanhamentoRegistrosPedagogicosInfantilComponenteDto
    {
        public RelatorioAcompanhamentoRegistrosPedagogicosInfantilComponenteDto()
        {
            Bimestres = new List<RelatorioAcompanhamentoRegistrosPedagogicosBimestreInfantilComponenteDto>();
        }

        public RelatorioAcompanhamentoRegistrosPedagogicosInfantilComponenteDto(List<RelatorioAcompanhamentoRegistrosPedagogicosBimestreInfantilComponenteDto> bimestres,
            RelatorioAcompanhamentoRegistrosPedagogicosCabecalhoDto cabecalho)
        {
            Bimestres = bimestres;
            Cabecalho = cabecalho;
        }

        public RelatorioAcompanhamentoRegistrosPedagogicosCabecalhoDto Cabecalho { get; set; }
        public List<RelatorioAcompanhamentoRegistrosPedagogicosBimestreInfantilComponenteDto> Bimestres { get; set; }

    }
}
