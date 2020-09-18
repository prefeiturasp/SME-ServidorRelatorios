using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterPlanoAulaFiltroQuery : IRequest<PlanoAulaDto>
    {
        public long PlanoAulaId { get; set; }
        public Usuario Usuario { get; set; }
    }
}
