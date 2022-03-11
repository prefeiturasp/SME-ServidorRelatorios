using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioAcompanhamentoRegistrosPedagogicosTurmaInfantilDiarioBordoComponenteDto
    {
        public string NomeTurma { get; set; }
        public List<RelatorioAcompanhamentoRegistrosPedagogicosDiarioBordoComponenteDto> Componentes { get; set; }
        public int Aulas { get; set; }
    }
}
