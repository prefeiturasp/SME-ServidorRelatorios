using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioCompensacaoAusenciaDto
    {
        public RelatorioCompensacaoAusenciaDto()
        {
        }
        public string DreNome { get; set; }
        public string UeNome { get; set; }        
        public string Bimestre { get; set; }
        public string ComponenteCurricular { get; set; }
        public string Usuario { get; set; }
        public string RF { get; set; }
        public string Data { get; set; }
        public string TurmaNome { get; set; }
        public string Modalidade { get; set; }        
        public RelatorioCompensacaoAusenciaDreDto Dre { get; set; }
    }
}
