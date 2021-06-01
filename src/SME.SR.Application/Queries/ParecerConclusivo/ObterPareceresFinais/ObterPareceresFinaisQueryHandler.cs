using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterPareceresFinaisQueryHandler : IRequestHandler<ObterPareceresFinaisQuery, IEnumerable<RelatorioParecerConclusivoRetornoDto>>
    {
        private readonly IParecerConclusivoRepository parecerConclusivoRepository;

        public ObterPareceresFinaisQueryHandler(IParecerConclusivoRepository usuarioRepository)
        {
            this.parecerConclusivoRepository = usuarioRepository ?? throw new ArgumentNullException(nameof(usuarioRepository));
        }

        public async Task<IEnumerable<RelatorioParecerConclusivoRetornoDto>> Handle(ObterPareceresFinaisQuery request, CancellationToken cancellationToken)
            => await parecerConclusivoRepository.ObterPareceresFinais(request.AnoLetivo, request.DreCodigo, request.UeCodigo, request.Modalidade, request.Semestre, request.CicloId, request.TurmasCodigo, request.Anos, request.ParecerConclusivoId);
    }
}
