using System;

namespace SME.SR.Infra
{
    public class RelatorioFrequenciaCabecalhoDto
    {
        public string Dre { get; set; }
        public string Ue { get; set; }
        public string Ano { get; set; }
        public string Bimestre { get; set; }
        public string Turma { get; set; }
        public string ComponenteCurricular { get; set; }
        public string Usuario { get; set; }
        public string RF { get; set; }
        public string Data => DateTime.Now.ToString("dd/MM/yyyy");
        public Modalidade Modalidade { get; set; }
    }
}
