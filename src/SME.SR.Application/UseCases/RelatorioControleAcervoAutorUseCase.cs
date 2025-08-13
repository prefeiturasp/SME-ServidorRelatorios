using MediatR;
using Sentry;
using SME.SR.Application.Commands.CDEP.GerarRelatorioControleAcervoAutor;
using SME.SR.Application.Interfaces.UseCases;
using SME.SR.Infra;
using SME.SR.Infra.Dtos.Relatorios.CDEP;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SME.SR.Application.UseCases
{
    public class RelatorioControleAcervoAutorUseCase : IRelatorioControleAcervoAutorUseCase
    {
        private readonly IMediator mediator;

        public RelatorioControleAcervoAutorUseCase(IMediator mediator)
        {
            this.mediator = mediator;
        }

        public async Task<MemoryStream> Executar(FiltroRelatorioSincronoDto request)
        {
            try
            {
                var filtros = request.ObterObjetoFiltro<FiltroRelatorioControleAcervoAutor>();

                return await mediator.Send(new GerarRelatorioControleAcervoAutorCommand(filtros));
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                throw ex;
            }
        }
    }
}
