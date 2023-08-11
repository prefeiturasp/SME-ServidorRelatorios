using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterObjetivosAprendizagemPlanejamentoAnualQuery : IRequest<IEnumerable<BimestreDescricaoObjetivosPlanoAnualDto>>
    {
        public ObterObjetivosAprendizagemPlanejamentoAnualQuery(long id)
        {
            Id = id;
        }
        
        public long Id { get; set; }
    }
}
