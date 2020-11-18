using MediatR;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterHistoricoReinicioSenhaUsuarioPorDreQueryHandler : IRequestHandler<ObterHistoricoReinicioSenhaUsuarioPorDreQuery, IEnumerable<HistoricoReinicioSenhaDto>>
    {
        private readonly IUsuarioRepository usuarioRepository;

        public ObterHistoricoReinicioSenhaUsuarioPorDreQueryHandler(IUsuarioRepository usuarioRepository)
        {
            this.usuarioRepository = usuarioRepository ?? throw new ArgumentNullException(nameof(usuarioRepository));
        }

        public async Task<IEnumerable<HistoricoReinicioSenhaDto>> Handle(ObterHistoricoReinicioSenhaUsuarioPorDreQuery request, CancellationToken cancellationToken)
        {
            var rfs = new List<string>();            

            var historicoReinicioSenha = await usuarioRepository.ObterHistoricoReinicioSenhaUsuarioPorDre(request.CodigoDre);            

            if (historicoReinicioSenha != null)
                foreach (var historico in historicoReinicioSenha)
                {
                    rfs.Add(historico.Rf);
                }

            // chamar query da base Eol passando lista de rf e retornando : rf, nome e perfil

            return historicoReinicioSenha;
        }
    }
}
