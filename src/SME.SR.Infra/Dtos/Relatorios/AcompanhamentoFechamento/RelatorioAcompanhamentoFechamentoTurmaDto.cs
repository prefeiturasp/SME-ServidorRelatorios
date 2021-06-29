using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioAcompanhamentoFechamentoTurmaDto
    {
        public RelatorioAcompanhamentoFechamentoTurmaDto()
        {
            Bimestres = new List<RelatorioAcompanhamentoFechamentoBimestreDto>();
        }

        public string TurmaDescricao { get; set; }

        public List<RelatorioAcompanhamentoFechamentoBimestreDto> Bimestres { get; set; }
    }
}
