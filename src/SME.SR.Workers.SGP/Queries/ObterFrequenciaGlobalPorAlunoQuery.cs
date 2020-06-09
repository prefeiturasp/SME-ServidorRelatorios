using MediatR;

namespace SME.SR.Workers.SGP.Queries
{
    public class ObterFrequenciaGlobalPorAlunoQuery : IRequest<double>
    {
        public string CodigoTurma { get; set; }
        public string CodigoAluno { get; set; }
    }
}
