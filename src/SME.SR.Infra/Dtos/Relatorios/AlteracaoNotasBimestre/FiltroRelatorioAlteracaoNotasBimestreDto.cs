using System;
using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class FiltroRelatorioAlteracaoNotasBimestreDto
    {
        public DateTime Data => DateTime.Now;
        public string CodigoDre { get; set; }
        public string CodigoUe { get; set; }
        public long AnoLetivo { get; set; }
        public Modalidade ModalidadeTurma { get; set; }
        public int Semestre { get; set; }
        public long Turma { get; set; }
        public IEnumerable<long> ComponentesCurriculares { get; set; }
        public List<int> Bimestres { get; set; }
        public TipoAlteracaoNota TipoAlteracaoNota { get; set; }
        public string NomeUsuario { get; set; }
    }
}
