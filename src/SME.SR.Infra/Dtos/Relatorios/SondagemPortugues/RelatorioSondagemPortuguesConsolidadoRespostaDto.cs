using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SME.SR.Infra
{
    public class RelatorioSondagemPortuguesConsolidadoRespostaDto
    {
        public string Id { get; set; }
        public string Resposta { get; set; }
        public int Quantidade { get; set; }
        public decimal Percentual { get; set; }
        public int Total { get; set; }
    }
}
