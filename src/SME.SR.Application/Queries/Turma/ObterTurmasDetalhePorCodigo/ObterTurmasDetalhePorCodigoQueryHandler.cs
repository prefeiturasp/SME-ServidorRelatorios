using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{ 
    public class ObterTurmasDetalhePorCodigoQueryHandler : IRequestHandler<ObterTurmasDetalhePorCodigoQuery, IEnumerable<Turma>>
    {
        private readonly ITurmaRepository turmaRepository;

        public ObterTurmasDetalhePorCodigoQueryHandler(ITurmaRepository turmaRepository)
        {
            this.turmaRepository = turmaRepository ?? throw new System.ArgumentNullException(nameof(turmaRepository));
        }

        public async Task<IEnumerable<Turma>> Handle(ObterTurmasDetalhePorCodigoQuery request, CancellationToken cancellationToken)
        {
            return await turmaRepository.ObterTurmasDetalhePorCodigos(request.TurmasCodigo);
        }
    }
  
}
