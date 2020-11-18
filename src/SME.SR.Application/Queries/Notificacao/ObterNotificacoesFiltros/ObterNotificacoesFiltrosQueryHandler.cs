using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterNotificacoesFiltrosQueryHandler : IRequestHandler<ObterNotificacoesFiltrosQuery, IEnumerable<NotificacaoDto>>
    {
        private readonly INotificacaoRepository notificacaoRepository;

        public ObterNotificacoesFiltrosQueryHandler(INotificacaoRepository notificacaoRepository)
        {
            this.notificacaoRepository = notificacaoRepository ?? throw new ArgumentNullException(nameof(notificacaoRepository));
        }

        public async Task<IEnumerable<NotificacaoDto>> Handle(ObterNotificacoesFiltrosQuery request, CancellationToken cancellationToken)
        {
            return await notificacaoRepository.ObterComFiltros(request.Ano, request.UsuarioRf, request.Categorias, request.Tipos, 
                request.Situacoes, request.ExibirDescricao, request.ExibirExcluidas, request.DRE, request.UE);
        }
    }
}
