using System;

namespace SME.SR.Infra
{
    public class CabecalhoPaginadoDto
    {
        public string NomeRelatorio { get; set; }
        public string Dre { get; set; }
        public string Ue { get; set; }
        public int AnoLetivo { get; set; }
        public string Ano { get; set; }
        public string Turma { get; set; }
        public string Bimestre { get; set; }
        public string ComponenteCurricular { get; set; }
        public string Proficiencia { get; set; }
        public string Periodo { get; set; }
        public string Usuario { get; set; }
        public string Rf { get; set; }
        public string DataSolicitacao { get => DateTime.Now.ToString("dd/MM/yyyy"); }
    }
}
