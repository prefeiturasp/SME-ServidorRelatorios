using System;

namespace SME.SR.Infra
{
    public class RelatorioRegistroIndividualCabecalhoDto
    {
        public string Dre { get; set; }
        public string Ue { get; set; }
        public string Endereco { get; set; }
        public string Telefone { get; set; }
        public string Turma { get; set; }
        public string Usuario { get; set; }
        public string RF { get; set; }
        public string Periodo { get; set; }
        public string Data => DateTime.Now.ToString("dd/MM/yyyy");
    }
}
