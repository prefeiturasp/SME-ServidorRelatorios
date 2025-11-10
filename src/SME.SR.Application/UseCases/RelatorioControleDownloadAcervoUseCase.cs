using MediatR;
using Sentry;
using SME.SR.Application.Commands.CDEP.GerarRelatorioControleDownloadAcervo;
using SME.SR.Application.Interfaces;
using SME.SR.Infra;
using SME.SR.Infra.Dtos.Relatorios.CDEP;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SME.SR.Application.UseCases
{
    public class RelatorioControleDownloadAcervoUseCase : IRelatorioControleDownloadAcervoUseCase
    {
        private readonly IMediator mediator;
        public RelatorioControleDownloadAcervoUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        public async Task<MemoryStream> Executar(FiltroRelatorioSincronoDto request)
        {
            try
            {
                var filtros = request.ObterObjetoFiltro<FiltroRelatorioControleDownloadAcervo>();
                return await mediator.Send(new GerarRelatorioControleDownloadAcervoCommand(filtros));
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                throw ex;
            }
        }
    }
}
