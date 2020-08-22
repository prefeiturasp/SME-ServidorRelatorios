using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioCompensacaoAusenciaBimestreDto
    {
        public RelatorioCompensacaoAusenciaBimestreDto()
        {
            Componentes = new List<RelatorioCompensacaoAusenciaComponenteDto>();
        }
        public string Nome { get; set; }

        public List<RelatorioCompensacaoAusenciaComponenteDto> Componentes { get; set; }

    }
}
