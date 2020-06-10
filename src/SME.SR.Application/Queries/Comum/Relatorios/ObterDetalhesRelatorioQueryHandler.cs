using MediatR;
using SME.SR.Infra.Dtos.Resposta;
using SME.SR.JRSClient.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application.Queries.Comum.Relatorios
{
    public class ObterDetalhesRelatorioQueryHandler : IRequestHandler<ObterDetalhesRelatorioQuery, DetalhesExecucaoRelatorioRespostaDto>
    {
        private readonly IExecucaoRelatorioService execucaoRelatorioService;

        public ObterDetalhesRelatorioQueryHandler(IExecucaoRelatorioService execucaoRelatorioService)
        {
            this.execucaoRelatorioService = execucaoRelatorioService ?? throw new ArgumentNullException(nameof(execucaoRelatorioService));
        }
        public async Task<DetalhesExecucaoRelatorioRespostaDto> Handle(ObterDetalhesRelatorioQuery request, CancellationToken cancellationToken)
        {
            return await execucaoRelatorioService.ObterDetalhes(request.RequisicaoId, request.JSessionId);
        }
    }
}
