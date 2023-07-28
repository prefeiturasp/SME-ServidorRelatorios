using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SME.SR.Infra;

namespace SME.SR.Data.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<Usuario> ObterDados(string codigoRf);
        Task<IEnumerable<DadosUsuarioDto>> ObterUsuariosAbrangenciaPorAcesso(string dreCodigo, string ueCodigo, string usuarioRf, Guid[] perfis, int diasSemAcesso);
        Task<SituacaoUsuario> ObterSituacaoUsuarioPorRf(string usuarioLogin);
        Task<string> ObterNomeUsuarioPorLogin(string usuarioLogin);
        Task<UsuarioCoreSSO> ObterDadosCoreSSO(string codigoRf);
        Task<IEnumerable<HistoricoReinicioSenhaDto>> ObterHistoricoReinicioSenhaUsuarioPorDre(string CodigoDre);
        Task<IEnumerable<PrioridadePerfil>> ObterListaPrioridadePerfil();
        Task<IEnumerable<Usuario>> ObterNomesUsuariosPorRfs(string[] codigosRfs);
    }
}
