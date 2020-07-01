using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SME.SR.Data.Interfaces;

namespace SME.SR.Application
{
    public class ObterCicloAprendizagemTurmaQueryHandler : IRequestHandler<ObterCicloAprendizagemTurmaQuery, string>
    {
        private ITurmaRepository turmaRepository;

        public ObterCicloAprendizagemTurmaQueryHandler(ITurmaRepository turmaRepository)
        {
            this.turmaRepository = turmaRepository ?? throw new ArgumentNullException(nameof(turmaRepository));
        }

        public async Task<string> Handle(ObterCicloAprendizagemTurmaQuery request, CancellationToken cancellationToken)
            => await turmaRepository.ObterCicloAprendizagem(request.TurmaCodigo);
    }
}
