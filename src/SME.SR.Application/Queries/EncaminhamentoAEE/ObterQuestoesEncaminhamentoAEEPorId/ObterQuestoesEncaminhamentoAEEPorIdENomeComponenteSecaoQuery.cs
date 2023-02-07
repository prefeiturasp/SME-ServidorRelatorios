using System.Collections.Generic;
using MediatR;
using SME.SR.Infra;

namespace SME.SR.Application
{
    public class ObterQuestoesEncaminhamentoAEEPorIdENomeComponenteSecaoQuery : IRequest<IEnumerable<QuestaoDto>>
    {
        public ObterQuestoesEncaminhamentoAEEPorIdENomeComponenteSecaoQuery(long encaminhamentoAEEId, string nomeComponenteSecao)
        {
            EncaminhamentoAeeId = encaminhamentoAEEId;
            NomeComponenteSecao = nomeComponenteSecao;
        }

        public long EncaminhamentoAeeId { get; }
        public string NomeComponenteSecao { get; }
    }
}