using MediatR;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SME.SR.Data.Interfaces;
using SME.SR.Data;
using System.Linq;

namespace SME.SR.Application
{
    public class ObterRelatorioSondagemComponentesPorTurmaQueryHandler : IRequestHandler<ObterRelatorioSondagemComponentesPorTurmaQuery, RelatorioSondagemComponentesPorTurmaRelatorioDto>
    { 
        private readonly IRelatorioSondagemComponentePorTurmaRepository relatorioSondagemComponentePorTurmaRepository;

        public ObterRelatorioSondagemComponentesPorTurmaQueryHandler(IRelatorioSondagemComponentePorTurmaRepository relatorioSondagemComponentePorTurmaRepository)
        {
            this.relatorioSondagemComponentePorTurmaRepository = relatorioSondagemComponentePorTurmaRepository ?? throw new ArgumentNullException(nameof(relatorioSondagemComponentePorTurmaRepository));
        }

        public Task<RelatorioSondagemComponentesPorTurmaRelatorioDto> Handle(ObterRelatorioSondagemComponentesPorTurmaQuery request, CancellationToken cancellationToken)
        {
            return this.relatorioSondagemComponentePorTurmaRepository.ObterRelatorio(request.DreId, request.TurmaId, request.UeId, request.Ano);
        }
    }
}
