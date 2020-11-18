using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterNotificacoesPorAnoEUsuarioRfQueryHandler : IRequestHandler<ObterNotificacoesPorAnoEUsuarioRfQuery, IEnumerable<NotificacaoDto>>
    {
        private readonly INotificacaoRepository notificacaoRepository;

        public ObterNotificacoesPorAnoEUsuarioRfQueryHandler(INotificacaoRepository notificacaoRepository)
        {
            this.notificacaoRepository = notificacaoRepository ?? throw new ArgumentNullException(nameof(notificacaoRepository));
        }

        public async Task<IEnumerable<NotificacaoDto>> Handle(ObterNotificacoesPorAnoEUsuarioRfQuery request, CancellationToken cancellationToken)
        {
            return await notificacaoRepository.ObterPorAnoEUsuarioRf(request.Ano, request.UsuarioRf);
        }
    }
}
