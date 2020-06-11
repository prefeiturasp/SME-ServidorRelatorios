using MediatR;
using SME.SR.Infra.Dtos.Resposta;
using System;

namespace SME.SR.Application.Queries.Comum.Relatorios
{
    public class ObterDetalhesRelatorioQuery : IRequest<DetalhesExecucaoRelatorioRespostaDto>
    {
        public ObterDetalhesRelatorioQuery(Guid requisicaoId, string jSessionId)
        {
            RequisicaoId = requisicaoId;
            JSessionId = jSessionId;
        }

        public Guid RequisicaoId { get; set; }
        public string JSessionId { get; }
    }
}
