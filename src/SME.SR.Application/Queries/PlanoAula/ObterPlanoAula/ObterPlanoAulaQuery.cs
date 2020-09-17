using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterPlanoAulaQuery : IRequest<IEnumerable<PlanoAulaDto>>
    {
        public ObterPlanoAulaQuery(long id)
        {
            PlanoAulaId = id;
        }
        public long PlanoAulaId { get; set; }

    }
}
