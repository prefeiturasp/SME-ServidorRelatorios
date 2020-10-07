using MediatR;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterTurmasPorUeAnosModalidadeSemestreQueryHandler : IRequestHandler<ObterTurmasPorUeAnosModalidadeSemestreQuery, IEnumerable<TurmaFiltradaUeCicloAnoDto>>
    {
        private readonly ITurmaRepository turmaRepository;

        public ObterTurmasPorUeAnosModalidadeSemestreQueryHandler(ITurmaRepository turmaRepository)
        {
            this.turmaRepository = turmaRepository ?? throw new ArgumentNullException(nameof(turmaRepository));
        }

        public async Task<IEnumerable<TurmaFiltradaUeCicloAnoDto>> Handle(ObterTurmasPorUeAnosModalidadeSemestreQuery request, CancellationToken cancellationToken)
        {
            var modalidadeId = request.Modalidade.HasValue ? (int)request.Modalidade : 0;
            return await turmaRepository.ObterTurmasPorUeAnosModalidadeESemestre(request.UesId, request.Anos, modalidadeId, request.Semestre );
        }
    }
}
