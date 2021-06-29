using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioAcompanhamentoFechamentoComponenteDto
    {
        public RelatorioAcompanhamentoFechamentoComponenteDto()
        {
            Pendencias = new List<string>();
        }

        public string Componente { get; set; }
        public string Status { get; set; }
        public List<string> Pendencias { get; set; }
    }
}
