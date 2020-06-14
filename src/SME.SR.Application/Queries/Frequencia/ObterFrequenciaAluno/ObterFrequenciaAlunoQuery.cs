using MediatR;
using SME.SR.Data;

namespace SME.SR.Application
{
    public class ObterFrequenciaAlunoQuery : IRequest<FrequenciaAluno>
    {
        public Turma Turma { get; set; }
        public string CodigoAluno { get; set; }
        public string ComponenteCurricularCodigo { get; set; }
        public PeriodoEscolar PeriodoEscolar { get; set; }
    }
}
