using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaPlanilhaDto
    {
        public List<RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaPlanilhaLinhasDto> Linhas;
        public RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaPlanilhaDto()
        {
            this.Linhas = new List<RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaPlanilhaLinhasDto>();
        }
    }
}
