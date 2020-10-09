using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterTurmaSondagemEolPorCodigoQueryHandler : IRequestHandler<ObterTurmaSondagemEolPorCodigoQuery, Turma>
    {
        private readonly ITurmaEolRepository turmaEolRepository;

        public ObterTurmaSondagemEolPorCodigoQueryHandler(ITurmaEolRepository turmaEolRepository)
        {
            this.turmaEolRepository = turmaEolRepository ?? throw new ArgumentNullException(nameof(turmaEolRepository));
        }
        public async Task<Turma> Handle(ObterTurmaSondagemEolPorCodigoQuery request, CancellationToken cancellationToken)
        {
            var turma = await turmaEolRepository.ObterTurmaSondagemPorCodigo(request.TurmaCodigo);
            if (turma is null)
                throw new NegocioException("Não foi possível obter a turma do Eol.");
            return turma;
        }
    }
}
