using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterPlanoAnualQuery : IRequest<PlanoAnualDto>
    {
        public ObterPlanoAnualQuery(long id)
        {
            Id = id;
        }
        
        public long Id { get; set; }
    }
}
