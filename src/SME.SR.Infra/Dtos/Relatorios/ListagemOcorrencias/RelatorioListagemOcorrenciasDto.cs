using System;
using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioListagemOcorrenciasDto
    {
        public string Dre { get; set; }
        public string Ue { get; set; }
        public string Usuario { get; set; }
        public DateTime DataSolicitacao { get; set; }

        public IEnumerable<RelatorioListagemOcorrenciasRegistroDto> Registro { get; set; }
    }
}
