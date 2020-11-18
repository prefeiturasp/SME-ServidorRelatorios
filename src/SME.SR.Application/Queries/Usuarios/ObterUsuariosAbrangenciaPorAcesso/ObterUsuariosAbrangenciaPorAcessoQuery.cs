using MediatR;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Application
{
    public class ObterUsuariosAbrangenciaPorAcessoQuery : IRequest<IEnumerable<DadosUsuarioDto>>
    {
        public ObterUsuariosAbrangenciaPorAcessoQuery(string dreCodigo, string ueCodigo, string usuarioRf, Guid[] perfis, int diasSemAcesso)
        {
            DreCodigo = dreCodigo;
            UeCodigo = ueCodigo;
            UsuarioRf = usuarioRf;
            Perfis = perfis;
            DiasSemAcesso = diasSemAcesso;
        }

        public string DreCodigo { get; set; }
        public string UeCodigo { get; set; }
        public string UsuarioRf { get; set; }
        public Guid[] Perfis { get; set; }
        public int DiasSemAcesso { get; set; }
    }
}
