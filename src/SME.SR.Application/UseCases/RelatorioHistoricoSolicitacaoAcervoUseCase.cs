using MediatR;
using Sentry;
using SME.SR.Application.Commands.CDEP.GerarRelatorioHistoricoSolicitacaoAcervo;
using SME.SR.Application.Interfaces.UseCases;
using SME.SR.Infra;
using SME.SR.Infra.Dtos.Relatorios.CDEP;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SME.SR.Application.UseCases
{
    public class RelatorioHistoricoSolicitacaoAcervoUseCase : IRelatorioHistoricoSolicitacaoAcervoUseCase
    {
        private readonly IMediator mediator;
        public RelatorioHistoricoSolicitacaoAcervoUseCase(IMediator mediator)
        {
            this.mediator = mediator;
        }
        public async Task<MemoryStream> Executar(FiltroRelatorioSincronoDto request)
        {
            try
            {
                var filtros = request.ObterObjetoFiltro<FiltroRelatorioHistoricoSolicitacaoAcervo>();
                return await mediator.Send(new GerarRelatorioHistoricoSolicitacaoAcervoCommand(filtros));
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                throw ex;
            }
        }
    }
}
