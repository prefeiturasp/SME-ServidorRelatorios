using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioAcompanhamentoFechamentoUesDto
    {
        public RelatorioAcompanhamentoFechamentoUesDto()
        {            
            Bimestres = new List<RelatorioAcompanhamentoFechamentoBimestresDto>();
        }

        public string NomeUe { get; set; }
        public List<RelatorioAcompanhamentoFechamentoBimestresDto> Bimestres { get; set; }
    }
}
