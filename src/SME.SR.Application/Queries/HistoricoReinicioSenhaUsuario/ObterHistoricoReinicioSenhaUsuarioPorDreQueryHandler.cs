using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterHistoricoReinicioSenhaUsuarioPorDreQueryHandler : IRequestHandler<ObterHistoricoReinicioSenhaUsuarioPorDreQuery, IEnumerable<HistoricoReinicioSenhaDto>>
    {
        private readonly IUsuarioRepository usuarioRepository;
        private readonly IMediator mediator;

        public ObterHistoricoReinicioSenhaUsuarioPorDreQueryHandler(IMediator mediator, IUsuarioRepository usuarioRepository)
        {
            this.usuarioRepository = usuarioRepository ?? throw new ArgumentNullException(nameof(usuarioRepository));
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<IEnumerable<HistoricoReinicioSenhaDto>> Handle(ObterHistoricoReinicioSenhaUsuarioPorDreQuery request, CancellationToken cancellationToken)
        {
            var historicoReinicioSenha = await usuarioRepository.ObterHistoricoReinicioSenhaUsuarioPorDre(request.CodigoDre);            

            if (historicoReinicioSenha != null)
            {
                var perfisPrioritarios = await mediator.Send(new ObterPerfisPrioritariosQuery());
                foreach (var historico in historicoReinicioSenha)
                {
                    historico.UtilizaSenhaPadao = await mediator.Send(new UsuarioPossuiSenhaPadraoQuery(historico.Rf)) ? "Sim" : "Não";
                    historico.Perfil = await ObterPerfilPrioritario(historico.Rf, perfisPrioritarios);
                }
            }

            return historicoReinicioSenha;
        }

        private async Task<string> ObterPerfilPrioritario(string usuarioRf, IEnumerable<PrioridadePerfil> perfisPrioritarios)
        {
            var perfisUsuarios = await mediator.Send(new ObterPerfisUsuarioPorRfQuery(usuarioRf));
            var perfisPriorizados = perfisPrioritarios.Where(c => perfisUsuarios.Contains(c.CodigoPerfil)).OrderBy(a => a.Ordem);

            return perfisPriorizados.FirstOrDefault()?.NomePerfil;
        }
    }
}
