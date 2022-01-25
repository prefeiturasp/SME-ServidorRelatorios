using System;

namespace SME.SR.Infra
{
    public class RelatorioAcompanhamentoFechamentoCabecalhoDto
    {
        public string DreNome { get; set; }
        public string UeNome { get; set; }
        public string Bimestre { get; set; }
        public string Turma { get; set; }
        public string Usuario { get; set; }
        public string RF { get; set; }
        public string Data => DateTime.Now.ToString("dd/MM/yyyy");
    }
}
