using MediatR;

namespace SME.SR.Workers.SGP.Queries
{
    public class ObterParecerConclusivoPorAlunoQuery : IRequest<string>
    {
        public long ConselhoClasseId { get; set; }
        public string CodigoAluno { get; set; }
    }
}
