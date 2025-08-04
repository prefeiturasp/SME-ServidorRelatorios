using MediatR;
using Sentry;
using SME.SR.Application.Commands.CDEP.GerarRelatorioControleLivrosEmprestadosSintetico;
using SME.SR.Application.Interfaces.UseCases;
using SME.SR.Infra;
using System;
using System.Threading.Tasks;

namespace SME.SR.Application.UseCases
{
    public class RelatorioControleLivrosEmprestadosSinteticoUseCase : IRelatorioControleLivrosEmprestadosUseCase
    {
        private readonly IMediator mediator;

        public RelatorioControleLivrosEmprestadosSinteticoUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<string> Executar(FiltroRelatorioSincronoDto request)
        {
            try
            {
                var filtros = request.ObterObjetoFiltro<FiltroRelatorioControleLivro>();

                switch (filtros.Modelo)
                {
                    case ModeloRelatorio.Sintetico:
                       return await mediator.Send(new GerarRelatorioControleLivrosEmprestadosSinteticoCommand(filtros));
                    //case ModeloRelatorio.Analitico:
                    //    await mediator.Send(new GerarRelatorioControleLivrosEmprestadosAnaliticoCommand(filtros));
                    //    break;
                    default:
                        break;
                }


            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                throw ex;
            }

            return string.Empty;
        }
    }
}
