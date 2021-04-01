using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application.Queries.BoletimEscolar
{
    public class ObterFehamentoPorCodigoTurmaQueryHandler : IRequestHandler<ObterFehamentoPorCodigoTurmaQuery, FechamentoTurma>
    {
        private readonly IFechamentoTurmaRepository fechamentoTurmaRepository;
        private readonly IMediator mediator;

        public ObterFehamentoPorCodigoTurmaQueryHandler(IFechamentoTurmaRepository fechamentoTurmaRepository,
                                                        IMediator mediator)
        {
            this.fechamentoTurmaRepository = fechamentoTurmaRepository ?? throw new System.ArgumentNullException(nameof(fechamentoTurmaRepository));
            this.mediator = mediator ?? throw new System.ArgumentNullException(nameof(mediator));
        }

        public async Task<FechamentoTurma> Handle(ObterFehamentoPorCodigoTurmaQuery request, CancellationToken cancellationToken)
        {
            var turma = await mediator.Send(new ObterTurmaQuery(request.CodigoTurma));

            if (turma == null)
                throw new NegocioException($"A turma com o código {request.CodigoTurma} não foi localizada.");

            var fechamento = (await fechamentoTurmaRepository.ObterFechamentosPorCodigosTurma(new string[] { request.CodigoTurma })).FirstOrDefault();

            if (fechamento == null)
                throw new NegocioException($"A turma {turma.Nome} não possui fechamento.");

            return fechamento;
        }
    }
}
