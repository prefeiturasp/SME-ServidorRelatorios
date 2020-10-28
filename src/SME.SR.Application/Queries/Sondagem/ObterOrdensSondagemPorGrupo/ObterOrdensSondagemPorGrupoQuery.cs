using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterOrdensSondagemPorGrupoQuery : IRequest<IEnumerable<SondagemOrdemDto>>
    {
        public GrupoSondagemEnum Grupo { get; set; }
    }
}
