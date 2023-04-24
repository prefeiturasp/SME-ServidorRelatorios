using MediatR;

namespace SME.SR.Application
{
    public class ObterFrequenciaGlobalPorAlunoQuery : IRequest<string>
    {
        public string CodigoTurma { get; set; }
        public string CodigoAluno { get; set; }
    }
}
