using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterNotificacoesFiltrosQueryHandler : IRequestHandler<ObterNotificacoesFiltrosQuery, IEnumerable<NotificacaoRetornoDto>>
    {
        private readonly INotificacaoRepository notificacaoRepository;

        public ObterNotificacoesFiltrosQueryHandler(INotificacaoRepository notificacaoRepository)
        {
            this.notificacaoRepository = notificacaoRepository ?? throw new ArgumentNullException(nameof(notificacaoRepository));
        }

        public async Task<IEnumerable<NotificacaoRetornoDto>> Handle(ObterNotificacoesFiltrosQuery request, CancellationToken cancellationToken)
            => await notificacaoRepository.ObterComFiltros(request.AnoLetivo, 
                                                           request.UsuarioRf, 
                                                           request.Categorias, 
                                                           request.Tipos,
                                                           request.Situacoes, 
                                                           request.TurmaCodigo,
                                                           request.DreCodigo,
                                                           request.UeCodigo,
                                                           request.ExibirDescricao, 
                                                           request.ExibirExcluidas);        
    }
}
