using MediatR;
using Sentry;
using SME.SR.Application.Commands.CDEP.GerarRelatorioControleLivrosEmprestadosAnalitico;
using SME.SR.Application.Commands.CDEP.GerarRelatorioControleLivrosEmprestadosSintetico;
using SME.SR.Application.Interfaces;
using SME.SR.Infra;
using SME.SR.Infra.Dtos.Relatorios.CDEP;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SME.SR.Application.UseCases
{
    public class RelatorioControleLivrosEmprestadosUseCase : IRelatorioControleLivrosEmprestadosUseCase
    {
        private readonly IMediator mediator;

        public RelatorioControleLivrosEmprestadosUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<MemoryStream> Executar(FiltroRelatorioSincronoDto request)
        {
            try
            {
                var filtros = request.ObterObjetoFiltro<FiltroRelatorioControleLivro>();

                switch (filtros.Modelo)
                {
                    case ModeloRelatorio.Sintetico:
                       return await mediator.Send(new GerarRelatorioControleLivrosEmprestadosSinteticoCommand(filtros));
                    case ModeloRelatorio.Analitico:
                        return await mediator.Send(new GerarRelatorioControleLivrosEmprestadosAnaliticoCommand(filtros));
                    default:
                        return await mediator.Send(new GerarRelatorioControleLivrosEmprestadosSinteticoCommand(filtros));
                }
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                throw ex;
            }
        }
    }
}
