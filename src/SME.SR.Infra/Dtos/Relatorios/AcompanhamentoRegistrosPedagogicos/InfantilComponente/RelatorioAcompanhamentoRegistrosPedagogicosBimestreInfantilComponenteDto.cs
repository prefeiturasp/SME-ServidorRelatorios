using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioAcompanhamentoRegistrosPedagogicosBimestreInfantilComponenteDto
    {
        public RelatorioAcompanhamentoRegistrosPedagogicosBimestreInfantilComponenteDto()
        {
            TurmasInfantilComponente = new List<RelatorioAcompanhamentoRegistrosPedagogicosTurmaInfantilComponenteDto>();
            TurmasInfantilDiarioBordoComponente = new List<RelatorioAcompanhamentoRegistrosPedagogicosTurmaInfantilDiarioBordoComponenteDto>();
        }

        public RelatorioAcompanhamentoRegistrosPedagogicosBimestreInfantilComponenteDto(string bimestre) : this()
        {
            Bimestre = bimestre;
        }

        public string Bimestre { get; set; }

        public List<RelatorioAcompanhamentoRegistrosPedagogicosTurmaInfantilComponenteDto> TurmasInfantilComponente { get; set; }
        public List<RelatorioAcompanhamentoRegistrosPedagogicosTurmaInfantilDiarioBordoComponenteDto> TurmasInfantilDiarioBordoComponente { get; set; }
    }
}
