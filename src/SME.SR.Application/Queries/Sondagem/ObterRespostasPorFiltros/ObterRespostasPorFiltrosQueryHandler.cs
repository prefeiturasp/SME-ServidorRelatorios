using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterRespostasPorFiltrosQueryHandler : IRequestHandler<ObterRespostasPorFiltrosQuery, IEnumerable<SondagemAutoralDto>>
    {
        private readonly ISondagemAutoralRepository sondagemAutoralRepository;

        public ObterRespostasPorFiltrosQueryHandler(ISondagemAutoralRepository sondagemAutoralRepository)
        {
            this.sondagemAutoralRepository = sondagemAutoralRepository ?? throw new ArgumentNullException(nameof(sondagemAutoralRepository));
        }

        public async Task<IEnumerable<SondagemAutoralDto>> Handle(ObterRespostasPorFiltrosQuery request, CancellationToken cancellationToken)
        {
            return await sondagemAutoralRepository.ObterPorFiltros(request.DreCodigo, request.UeCodigo, request.GrupoId, request.PeriodoId, request.Bimestre, request.TurmaAno, request.AnoLetivo, request.ComponenteCurricular);
        }
    }
}
