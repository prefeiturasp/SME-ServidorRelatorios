using System;
using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioListagemOcorrenciasDto
    {
        public string Usuario { get; set; }
        public DateTime DataSolicitacao { get; set; }
        public bool ImprimirDescricaoOcorrencia { get; set; }
        public IEnumerable<UeDto> Ues { get; set; }
        public IEnumerable<RelatorioListagemOcorrenciasRegistroDto> Registros { get; set; }
    }
}
