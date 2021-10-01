using SME.SR.Infra.Utilitarios;
using System;

namespace SME.SR.Infra
{
    public class RegistroFrequenciaAlunoDto
    {
        public long CodigoAluno { get; set; }
        public long ComponenteCurricularId { get; set; }
        public TipoFrequencia TipoFrequencia { get; set; }
        public string TurmaCodigo { get; set; }
        public int AnoTurma { get; set; }
        public int ModalidadeTurma { get; set; }
        public DateTime PeriodoInicio { get; set; }
        public DateTime PeriodoFim { get; set; }
        public int Bimestre { get; set; }
        public int TotalAulas { get; set; }

    }
}
