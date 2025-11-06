using MediatR;
using Sentry;
using SME.SR.Application.Commands.CDEP.GerarRelatorioTitulosMaisPesquisados;
using SME.SR.Application.Interfaces;
using SME.SR.Infra;
using SME.SR.Infra.Dtos.Relatorios.CDEP;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SME.SR.Application.UseCases
{
    public class RelatorioTitulosMaisPesquisadosUseCase : IRelatorioTitulosMaisPesquisadosUseCase
    {
        private readonly IMediator mediator;
        public RelatorioTitulosMaisPesquisadosUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        public async Task<MemoryStream> Executar(FiltroRelatorioSincronoDto request)
        {
            try
            {
                var filtros = request.ObterObjetoFiltro<FiltroRelatorioTitulosMaisPesquisados>();
                return await mediator.Send(new GerarRelatorioTitulosMaisPesquisadosCommand(filtros));
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                throw ex;
            }
        }
    }
}
