using MediatR;
using SME.SR.Workers.SGP.Models;

namespace SME.SR.Workers.SGP.Queries
{
    public class ObterFrequenciaAlunoQuery : IRequest<FrequenciaAluno>
    {
        public Turma Turma { get; set; }
        public string CodigoAluno { get; set; }
        public string ComponenteCurricularCodigo { get; set; }
        public PeriodoEscolar PeriodoEscolar { get; set; }
    }
}
