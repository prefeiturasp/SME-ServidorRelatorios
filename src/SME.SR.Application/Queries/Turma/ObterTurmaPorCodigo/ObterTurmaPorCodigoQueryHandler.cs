using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterTurmaPorCodigoQueryHandler : IRequestHandler<ObterTurmaPorCodigoQuery, Turma>
    {
        private readonly ITurmaRepository turmaRepository;

        public ObterTurmaPorCodigoQueryHandler(ITurmaRepository turmaRepository)
        {
            this.turmaRepository = turmaRepository ?? throw new ArgumentNullException(nameof(turmaRepository));
        }

        public async Task<Turma> Handle(ObterTurmaPorCodigoQuery request, CancellationToken cancellationToken)
            => await turmaRepository.ObterPorCodigo(request.TurmaCodigo);
    }
}
