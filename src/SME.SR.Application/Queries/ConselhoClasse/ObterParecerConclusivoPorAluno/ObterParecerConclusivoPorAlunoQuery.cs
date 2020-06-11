using MediatR;

namespace SME.SR.Application
{
    public class ObterParecerConclusivoPorAlunoQuery : IRequest<string>
    {
        public long ConselhoClasseId { get; set; }
        public string CodigoAluno { get; set; }
    }
}
