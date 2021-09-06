using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioAcompanhamentoFechamentoComponenteDto
    {
        public RelatorioAcompanhamentoFechamentoComponenteDto(string componente, string status, List<string> pendencias)
        {
            Componente = componente;
            Status = status;
            Pendencias = new List<string>();
            Pendencias = pendencias;
        }

        public string Componente { get; set; }
        public string Status { get; set; }
        public List<string> Pendencias { get; set; }
    }
}
