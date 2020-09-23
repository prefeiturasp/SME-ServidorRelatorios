using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterPlanoAulaObjetivoAprendizagemQuery : IRequest<IEnumerable<ObjetivoAprendizagemDto>>
    {
        public ObterPlanoAulaObjetivoAprendizagemQuery(long id)
        {
            PlanoAulaId = id;
        }
        public long PlanoAulaId { get; set; }

    }
}
