using MediatR;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SME.SR.Data.Interfaces;
using SME.SR.Data;

namespace SME.SR.Application
{
    public class ObterRelatorioSondagemComponentesPorTurmaQueryHandler : IRequestHandler<ObterRelatorioSondagemComponentesPorTurmaQuery, IEnumerable<RelatorioSondagemComponentesPorTurmaRetornoQueryDto>>
    { 
        private readonly IRelatorioSondagemComponentePorTurmaRepository relatorioSondagemComponentePorTurmaRepository;

        public ObterRelatorioSondagemComponentesPorTurmaQueryHandler(IRelatorioSondagemComponentePorTurmaRepository relatorioSondagemComponentePorTurmaRepository)
        {
            this.relatorioSondagemComponentePorTurmaRepository = relatorioSondagemComponentePorTurmaRepository ?? throw new ArgumentNullException(nameof(relatorioSondagemComponentePorTurmaRepository));
        }

        async Task<IEnumerable<RelatorioSondagemComponentesPorTurmaRetornoQueryDto>> IRequestHandler<ObterRelatorioSondagemComponentesPorTurmaQuery, IEnumerable<RelatorioSondagemComponentesPorTurmaRetornoQueryDto>>.Handle(ObterRelatorioSondagemComponentesPorTurmaQuery request, CancellationToken cancellationToken)
        {
            return await relatorioSondagemComponentePorTurmaRepository.ObterRelatorio(request.DreId, request.TurmaId, request.UeId, request.Ano);
        }
    }
}
