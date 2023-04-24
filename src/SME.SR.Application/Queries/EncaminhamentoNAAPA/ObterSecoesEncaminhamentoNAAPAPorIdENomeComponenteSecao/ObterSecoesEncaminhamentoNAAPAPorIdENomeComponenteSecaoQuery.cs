using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterSecoesEncaminhamentoNAAPAPorIdENomeComponenteSecaoQuery : IRequest<IEnumerable<SecaoEncaminhamentoNAAPADto>>
    {
        public ObterSecoesEncaminhamentoNAAPAPorIdENomeComponenteSecaoQuery(long encaminhamentoNaapaId, string nomeComponenteSecao)
        {
            EncaminhamentoNaapaId = encaminhamentoNaapaId;
            NomeComponenteSecao = nomeComponenteSecao;
        }

        public long EncaminhamentoNaapaId { get; }
        public string NomeComponenteSecao { get; }
    }
}
