using MediatR;
using System;

namespace SME.SR.Application.Commands.RetornarRelatorioPronto
{
    public class RetornarRelatorioProntoCommand : IRequest<bool>
    {
        public RetornarRelatorioProntoCommand(Guid requisicaoId, Guid exportacaoId, Guid correlacaoId, string jSessionId)
        {
            RequisicaoId = requisicaoId;
            ExportacaoId = exportacaoId;
            CorrelacaoId = correlacaoId;
            JSessionId = jSessionId;
        }

        public Guid RequisicaoId { get; set; }
        public Guid ExportacaoId { get; set; }
        public Guid CorrelacaoId { get; set; }
        public string JSessionId { get; }
    }
}
