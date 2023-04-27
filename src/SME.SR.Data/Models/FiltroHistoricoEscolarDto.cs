using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Data
{
    public class FiltroHistoricoEscolarDto
    {
        public int AnoLetivo { get; set; }
        public string DreCodigo { get; set; }
        public string UeCodigo { get; set; }
        public Modalidade Modalidade { get; set; }
        public string TurmaCodigo { get ; set; }
        public IEnumerable<FiltroHistoricoEscolarAlunosDto> Alunos { get; set; }
        public string[] AlunosCodigo { get; set; }
        public bool ImprimirDadosResponsaveis { get; set; }
        public bool PreencherDataImpressao { get; set; }
        public int Semestre { get; set; }
        public Usuario Usuario { get; set; }
    }
}
