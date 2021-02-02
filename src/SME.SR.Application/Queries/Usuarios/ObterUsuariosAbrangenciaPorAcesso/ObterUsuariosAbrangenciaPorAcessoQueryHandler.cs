using MediatR;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterUsuariosAbrangenciaPorAcessoQueryHandler : IRequestHandler<ObterUsuariosAbrangenciaPorAcessoQuery, IEnumerable<DadosUsuarioDto>>
    {
        private readonly IUsuarioRepository usuarioRepository;

        public ObterUsuariosAbrangenciaPorAcessoQueryHandler(IUsuarioRepository usuarioRepository)
        {
            this.usuarioRepository = usuarioRepository ?? throw new ArgumentNullException(nameof(usuarioRepository));
        }


        public async Task<IEnumerable<DadosUsuarioDto>> Handle(ObterUsuariosAbrangenciaPorAcessoQuery request, CancellationToken cancellationToken)
            => await usuarioRepository.ObterUsuariosAbrangenciaPorAcesso(request.DreCodigo,
                                                                         request.UeCodigo,
                                                                         request.UsuarioRf,
                                                                         request.Perfis,
                                                                         request.DiasSemAcesso);
    }
}
