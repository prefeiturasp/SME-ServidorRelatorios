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
    public class ObterRelatorioSondagemPortuguesPorTurmaQueryHandler : IRequestHandler<ObterRelatorioSondagemPortuguesPorTurmaQuery, IEnumerable<RelatorioSondagemPortuguesPorTurmaPlanilhaQueryDto>>
    {
        private readonly IRelatorioSondagemPortuguesPorTurmaRepository relatorioSondagemPortuguesPorTurmaRepository;
        private readonly IMediator mediator;

        public ObterRelatorioSondagemPortuguesPorTurmaQueryHandler(
            IRelatorioSondagemPortuguesPorTurmaRepository relatorioSondagemPortuguesPorTurmaRepository,
            IMediator mediator)
        {
            this.relatorioSondagemPortuguesPorTurmaRepository = relatorioSondagemPortuguesPorTurmaRepository ?? throw new ArgumentNullException(nameof(relatorioSondagemPortuguesPorTurmaRepository));
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<IEnumerable<RelatorioSondagemPortuguesPorTurmaPlanilhaQueryDto>> Handle(ObterRelatorioSondagemPortuguesPorTurmaQuery request, CancellationToken cancellationToken)
        {
            return await relatorioSondagemPortuguesPorTurmaRepository.ObterPlanilhaLinhas(request.DreCodigo, request.UeCodigo, request.TurmaCodigo, request.AnoLetivo, request.AnoTurma, request.Bimestre, request.Proficiencia);
        }
    }
}
