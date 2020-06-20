using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Data
{
    public class AtribuicaoCJ
    {
        public long DisciplinaId { get; set; }
        public string DreId { get; set; }
        public bool Migrado { get; set; }
        public Modalidade Modalidade { get; set; }
        public string ProfessorRf { get; set; }
        public bool Substituir { get; set; }
        public Turma Turma { get; set; }
        public string TurmaId { get; set; }
        public string UeId { get; set; }
    }
}
