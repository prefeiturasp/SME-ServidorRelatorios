using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterPareceresFinaisConsolidadoQueryHandler : IRequestHandler<ObterPareceresFinaisConsolidadoQuery, IEnumerable<RelatorioParecerConclusivoRetornoDto>>
    {
        private readonly IParecerConclusivoRepository parecerConclusivoRepository;

        public ObterPareceresFinaisConsolidadoQueryHandler(IParecerConclusivoRepository usuarioRepository)
        {
            this.parecerConclusivoRepository = usuarioRepository ?? throw new ArgumentNullException(nameof(usuarioRepository));
        }

        public async Task<IEnumerable<RelatorioParecerConclusivoRetornoDto>> Handle(ObterPareceresFinaisConsolidadoQuery request, CancellationToken cancellationToken)
            => await parecerConclusivoRepository.ObterPareceresFinaisConsolidado(request.AnoLetivo, request.DreCodigo, request.UeCodigo, request.Modalidade, request.Semestre, request.TurmasCodigo, request.Anos, request.ParecerConclusivoId);
    }
}
