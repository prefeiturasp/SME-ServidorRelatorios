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
    public class ObterRelatorioSondagemPortuguesPorTurmaQueryHandler : IRequestHandler<ObterRelatorioSondagemPortuguesPorTurmaQuery, RelatorioSondagemPortuguesPorTurmaRelatorioDto>
    {
        private readonly IRelatorioSondagemPortuguesPorTurmaRepository relatorioSondagemPortuguesPorTurmaRepository;
        private readonly IUsuarioRepository usuarioRepository;
        private readonly IMediator mediator;

        public ObterRelatorioSondagemPortuguesPorTurmaQueryHandler(
            IRelatorioSondagemPortuguesPorTurmaRepository relatorioSondagemPortuguesPorTurmaRepository,
            IUsuarioRepository usuarioRepository,
            IMediator mediator)
        {
            this.relatorioSondagemPortuguesPorTurmaRepository = relatorioSondagemPortuguesPorTurmaRepository ?? throw new ArgumentNullException(nameof(relatorioSondagemPortuguesPorTurmaRepository));
            this.usuarioRepository = usuarioRepository ?? throw new ArgumentNullException(nameof(usuarioRepository));
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<RelatorioSondagemPortuguesPorTurmaRelatorioDto> Handle(ObterRelatorioSondagemPortuguesPorTurmaQuery request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
