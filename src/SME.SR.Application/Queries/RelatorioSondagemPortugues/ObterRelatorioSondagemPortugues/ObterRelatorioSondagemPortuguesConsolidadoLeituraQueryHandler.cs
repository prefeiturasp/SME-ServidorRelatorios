using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using SME.SR.Infra.Utilitarios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace SME.SR.Application
{
    public class ObterRelatorioSondagemPortuguesConsolidadoLeituraQueryHandler : IRequestHandler<ObterRelatorioSondagemPortuguesConsolidadoLeituraQuery, IEnumerable<RelatorioSondagemPortuguesConsolidadoLeituraPlanilhaQueryDto>>
    {
        private readonly IRelatorioSondagemPortuguesConsolidadoLeituraRepository relatorioSondagemPortuguesConsolidadoLeituraRepository;
        private readonly IMediator mediator;

        public ObterRelatorioSondagemPortuguesConsolidadoLeituraQueryHandler(
            IRelatorioSondagemPortuguesConsolidadoLeituraRepository relatorioSondagemPortuguesConsolidadoLeituraRepository,
            IMediator mediator)
        {
            this.relatorioSondagemPortuguesConsolidadoLeituraRepository = relatorioSondagemPortuguesConsolidadoLeituraRepository ?? throw new ArgumentNullException(nameof(relatorioSondagemPortuguesConsolidadoLeituraRepository));
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        public async Task<IEnumerable<RelatorioSondagemPortuguesConsolidadoLeituraPlanilhaQueryDto>> Handle(ObterRelatorioSondagemPortuguesConsolidadoLeituraQuery request, CancellationToken cancellationToken)
        {
            return await relatorioSondagemPortuguesConsolidadoLeituraRepository.ObterPlanilhas(request.DreCodigo, request.UeCodigo, request.TurmaCodigo, request.AnoLetivo, request.AnoTurma, request.Bimestre);
        }
    }
}
