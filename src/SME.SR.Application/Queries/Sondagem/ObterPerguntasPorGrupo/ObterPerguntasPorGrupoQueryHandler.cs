using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterPerguntasPorGrupoQueryHandler : IRequestHandler<ObterPerguntasPorGrupoQuery, IEnumerable<PerguntasOrdemGrupoAutoralDto>>
    {
        private readonly IPerguntasAutoralRepository perguntasAutoralRepository;

        public ObterPerguntasPorGrupoQueryHandler(IPerguntasAutoralRepository perguntasAutoralRepository)
        {
            this.perguntasAutoralRepository = perguntasAutoralRepository ?? throw new System.ArgumentNullException(nameof(perguntasAutoralRepository));
        }

        public async Task<IEnumerable<PerguntasOrdemGrupoAutoralDto>> Handle(ObterPerguntasPorGrupoQuery request, CancellationToken cancellationToken)
        {
           return await perguntasAutoralRepository.ObterPerguntasPorGrupo(request.GrupoSondagem, request.ComponenteCurricular);
        }
    }
}
