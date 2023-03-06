using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore.Internal;
using SME.SR.Application.Interfaces;
using SME.SR.Infra;
using SME.SR.Infra.EncaminhamentoNaapa;

namespace SME.SR.Application
{
    public class RelatorioEncaminhamentosNaapaUseCase : IRelatorioEncaminhamentosNaapaUseCase
    {
        private readonly IMediator mediator;

        public RelatorioEncaminhamentosNaapaUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Executar(FiltroRelatorioDto request)
        {
            var filtroRelatorio = request.ObterObjetoFiltro<FiltroRelatorioEncaminhamentoNaapaDto>();
            
            if (filtroRelatorio == null || !filtroRelatorio.EncaminhamentoNaapaIds.Any())
                throw new NegocioException("Nenhum Id de Encaminhamento NAAPA informado para realizar a Consulta");
        }
    }
}