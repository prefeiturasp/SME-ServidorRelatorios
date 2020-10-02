using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra.Dtos.Relatorios.Sondagem
{
    public class RelatorioDto
    {
        [JsonProperty("cabecalho")]
        public CabecalhoDto Cabecalho { get; set; }

        [JsonProperty("conteudo")]
        public PlanilhaDto Planilha { get; set;  } 
    }
}
