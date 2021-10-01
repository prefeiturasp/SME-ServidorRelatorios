using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioFaltasFrequenciaDto
    {
        public RelatorioFaltasFrequenciaDto()
        {
            Dres = new List<RelatorioFrequenciaDreDto>();
        }
        public bool ExibeFaltas { get; set; }
        public bool ExibeFrequencia { get; set; }
        public string Dre { get; set; }
        public string Ue{ get; set; }
        public string Ano{ get; set; }
        public string Bimestre{ get; set; }
        public string Turma{ get; set; }
        public string ComponenteCurricular{ get; set; }
        public string Usuario{ get; set; }
        public string RF{ get; set; }
        public string Data{ get; set; }
        public string Modalidade { get; set; }
        public string UltimoAluno { get; set; }
        public List<RelatorioFrequenciaDreDto> Dres { get; set; }
        public string TurmaAno { 
            get {
                return $"{Turma}-{Ano}";
            }
        }
    }
}
