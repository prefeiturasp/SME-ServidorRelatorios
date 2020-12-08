using SME.SR.Infra;
using System;

namespace SME.SR.Data
{
    public class AtribuicaoCJ
    {
        public long ComponenteCurricularId { get; set; }
        public string ComponenteCurricularNome { get; set; }
        public DateTime CriadoEm { get; set; }
        public string DreId { get; set; }
        public bool Migrado { get; set; }
        public Modalidade Modalidade { get; set; }
        public string ProfessorRf { get; set; }
        public string ProfessorNome { get; set; }
        public bool Substituir { get; set; }
        public Turma Turma { get; set; }
        public string TurmaId { get; set; }
        public string UeId { get; set; }
    }
}
