using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterTurmaQueryHandler : IRequestHandler<ObterTurmaQuery, Turma>
    {
        private readonly ITurmaRepository turmaSgpRepository;

        public ObterTurmaQueryHandler(ITurmaRepository turmaSgpRepository)
        {
            this.turmaSgpRepository = turmaSgpRepository ?? throw new ArgumentNullException(nameof(turmaSgpRepository));
        }

        public async Task<Turma> Handle(ObterTurmaQuery request, CancellationToken cancellationToken)
        {
            var turma = await turmaSgpRepository.ObterComDreUePorCodigo(request.CodigoTurma);
            if (turma == null)
            {
                throw new NegocioException("Não foi possível localizar a turma.");
            }

            return turma;
        }
    }
}
