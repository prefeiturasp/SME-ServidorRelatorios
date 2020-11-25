using MediatR;
using SME.SR.Application.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class RelatorioAlteracaoNotasBimestreUseCase : IRelatorioAlteracaoNotasBimestreUseCase
    {
        private readonly IMediator mediator;

        public RelatorioAlteracaoNotasBimestreUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Executar(FiltroRelatorioDto request)
        {
            try
            {
                var filtros = request.ObterObjetoFiltro<FiltroRelatorioAlteracaoNotasBimestreDto>();
                await mediator.Send(new GerarRelatorioAlteracaoNotasBimestreCommand(filtros, request.CodigoCorrelacao));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
