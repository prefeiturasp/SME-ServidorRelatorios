using System.Collections.Generic;
using MediatR;
using SME.SR.Infra;

namespace SME.SR.Application
{
    public class ObterRespostasEncaminhamentoAEEPorIdENomeComponenteSecaoQuery : IRequest<IEnumerable<RespostaQuestaoDto>>
    {
        public ObterRespostasEncaminhamentoAEEPorIdENomeComponenteSecaoQuery(long encaminhamentoAeeId, string nomeComponenteSecao)
        {
            EncaminhamentoAeeId = encaminhamentoAeeId;
            NomeComponenteSecao = nomeComponenteSecao;
        }

        public long EncaminhamentoAeeId { get; }
        public string NomeComponenteSecao { get; }
    }
}