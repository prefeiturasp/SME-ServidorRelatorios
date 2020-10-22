using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class RelatorioSondagemPortuguesConsolidadoLeituraPlanilhaQueryDto
    {
        public string Ordem { get; set; }
        public string Pergunta { get; set; }
        public string Resposta { get; set; }
        public int Quantidade { get; set; }
    }
}
