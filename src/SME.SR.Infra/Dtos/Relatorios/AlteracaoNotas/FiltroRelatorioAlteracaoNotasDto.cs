using System;
using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class FiltroRelatorioAlteracaoNotasDto
    {
        public DateTime Data => DateTime.Now;
        public string CodigoDre { get; set; }
        public string CodigoUe { get; set; }
        public int AnoLetivo { get; set; }
        public Modalidade ModalidadeTurma { get; set; }
        public int Semestre { get; set; }
        public IEnumerable<long> Turma { get; set; }
        public long[] ComponentesCurriculares { get; set; }
        public int[] Bimestres { get; set; }
        public TipoAlteracaoNota TipoAlteracaoNota { get; set; }
        public string NomeUsuario { get; set; }
    }
}
