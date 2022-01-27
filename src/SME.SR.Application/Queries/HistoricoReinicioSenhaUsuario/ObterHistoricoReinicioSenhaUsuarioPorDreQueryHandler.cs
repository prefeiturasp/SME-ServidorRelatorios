using MediatR;
using Sentry;
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
            try
            {
                SentrySdk.CaptureMessage("1.10 ObterHistoricoReinicioSenhaUsuarioPorDre - ObterHistoricoReinicioSenhaUsuarioPorDreQueryHandler");
                var historicoReinicioSenha = await usuarioRepository.ObterHistoricoReinicioSenhaUsuarioPorDre(request.CodigoDre);
                if (historicoReinicioSenha != null)
                {
                    var perfisPrioritarios = await mediator.Send(new ObterPerfisPrioritariosQuery());
                    foreach (var historico in historicoReinicioSenha)
                    {
                        SentrySdk.CaptureMessage("1.11 UsuarioPossuiSenhaPadraoQuery - UsuarioPossuiSenhaPadraoQuery");
                        historico.UtilizaSenhaPadao = await mediator.Send(new UsuarioPossuiSenhaPadraoQuery(historico.Login)) ? "Sim" : "Não";

                        SentrySdk.CaptureMessage("1.12 ObterPerfilPrioritario - UsuarioPossuiSenhaPadraoQuery");
                        historico.Perfil = await ObterPerfilPrioritario(historico.Login, perfisPrioritarios);

                        if (string.IsNullOrEmpty(historico.Nome) && !string.IsNullOrEmpty(historico.Perfil))
                        {
                            SentrySdk.CaptureMessage("1.13 ObterDadosUsuarioCoreSSOPorRfQuery - UsuarioPossuiSenhaPadraoQuery");
                            var usuarioCoreSSO = await mediator.Send(new ObterDadosUsuarioCoreSSOPorRfQuery(historico.Login));
                            historico.Nome = usuarioCoreSSO.Nome;
                        }

                        if (!string.IsNullOrEmpty(historico.Perfil))
                            historico.SenhaReiniciadaPorPerfil = await ObterPerfilPrioritario(historico.SenhaReiniciadaPorRf, perfisPrioritarios);
                    }
                }

                return historicoReinicioSenha.Where(c => !string.IsNullOrEmpty(c.Perfil));
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                throw;
            }            
        }

        private async Task<string> ObterPerfilPrioritario(string usuarioRf, IEnumerable<PrioridadePerfil> perfisPrioritarios)
        {
            SentrySdk.CaptureMessage("1.14 ObterPerfilPrioritario - UsuarioPossuiSenhaPadraoQuery");
            var perfisUsuarios = await mediator.Send(new ObterPerfisUsuarioPorRfQuery(usuarioRf));
            var perfisPriorizados = perfisPrioritarios.Where(c => perfisUsuarios.Contains(c.CodigoPerfil)).OrderBy(a => a.Ordem);

            return perfisPriorizados.FirstOrDefault()?.NomePerfil;
        }
    }
}
