using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioCompensacaoAusenciaComponenteDto
    {
        public RelatorioCompensacaoAusenciaComponenteDto()
        {
            Atividades = new List<RelatorioCompensacaoAusenciaAtividadeDto>();
        }
        public string NomeComponente { get; set; }
        public string CodigoComponente { get; set; }
        public List<RelatorioCompensacaoAusenciaAtividadeDto> Atividades { get; set; }

    }
}

