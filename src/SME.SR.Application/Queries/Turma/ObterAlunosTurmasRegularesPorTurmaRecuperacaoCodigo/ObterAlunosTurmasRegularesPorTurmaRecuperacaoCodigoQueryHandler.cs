using MediatR;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterAlunosTurmasRegularesPorTurmaRecuperacaoCodigoQueryHandler : IRequestHandler<ObterAlunosTurmasRegularesPorTurmaRecuperacaoCodigoQuery, IEnumerable<AlunoTurmaRegularRetornoDto>>
    {
        private readonly ITurmaRepository turmaRepository;

        public ObterAlunosTurmasRegularesPorTurmaRecuperacaoCodigoQueryHandler(ITurmaRepository turmaRepository)
        {
            this.turmaRepository = turmaRepository ?? throw new ArgumentNullException(nameof(turmaRepository));
        }

        public async Task<IEnumerable<AlunoTurmaRegularRetornoDto>> Handle(ObterAlunosTurmasRegularesPorTurmaRecuperacaoCodigoQuery request, CancellationToken cancellationToken)
        {
            return await turmaRepository.ObterAlunosTurmasRegularesPorTurmaRecuperacaoCodigoQuery(request.TurmaCodigo);
        }
    }
}
