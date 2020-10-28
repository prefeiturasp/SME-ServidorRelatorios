using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using SME.SR.Infra.Utilitarios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterOrdensSondagemPorGrupoQueryHandler : IRequestHandler<ObterOrdensSondagemPorGrupoQuery, IEnumerable<SondagemOrdemDto>>
    {
        private readonly ISondagemOrdemRepository sondagemOrdemRepository;

        public ObterOrdensSondagemPorGrupoQueryHandler(
            ISondagemOrdemRepository sondagemOrdemRepository)
        {
            this.sondagemOrdemRepository = sondagemOrdemRepository ?? throw new ArgumentNullException(nameof(sondagemOrdemRepository));
        }

        public async Task<IEnumerable<SondagemOrdemDto>> Handle(ObterOrdensSondagemPorGrupoQuery request, CancellationToken cancellationToken)
        {
            return await sondagemOrdemRepository.ObterPorGrupo(request.Grupo);
        }
    }
}
