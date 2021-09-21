using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterAulasDadasTurmaBimestreComponenteCurricularQueryHandler : IRequestHandler<ObterAulasDadasTurmaBimestreComponenteCurricularQuery, IEnumerable<TurmaComponenteQuantidadeAulasDto>>
    {
        private readonly IAulaPrevistaBimestreRepository aulaPrevistaBimestreRepository;

        public ObterAulasDadasTurmaBimestreComponenteCurricularQueryHandler(IAulaPrevistaBimestreRepository aulaPrevistaBimestreRepository)
        {
            this.aulaPrevistaBimestreRepository = aulaPrevistaBimestreRepository ?? throw new ArgumentNullException(nameof(aulaPrevistaBimestreRepository));
        }

        public async Task<IEnumerable<TurmaComponenteQuantidadeAulasDto>> Handle(ObterAulasDadasTurmaBimestreComponenteCurricularQuery request, CancellationToken cancellationToken)
        {
            return await aulaPrevistaBimestreRepository
                .ObterBimestresAulasTurmasComponentesCumpridasAsync(request.TurmasCodigo,
                                                                    request.ComponentesCurricularesId,
                                                                    request.TipoCalendarioId);
        }
    }
}
