using MediatR;

namespace SME.SR.Application
{
    public class ObterFrequenciaGlobalPorAlunoQuery : IRequest<double?>
    {
        public string CodigoTurma { get; set; }
        public string CodigoAluno { get; set; }
    }
}
