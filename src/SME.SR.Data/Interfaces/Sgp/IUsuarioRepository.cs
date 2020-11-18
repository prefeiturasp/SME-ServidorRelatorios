using System.Collections.Generic;
using System.Threading.Tasks;
using SME.SR.Infra;

namespace SME.SR.Data.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<Usuario> ObterDados(string codigoRf);
        Task<IEnumerable<DadosUsuarioDto>> ObterUsuariosAbrangenciaPorAcesso(string dreCodigo, string ueCodigo, string usuarioRf, string[] perfis, int diasSemAcesso);
        Task<SituacaoUsuario> ObterSituacaoUsuarioPorRf(string usuarioRf);
        Task<UsuarioCoreSSO> ObterDadosCoreSSO(string codigoRf);
        Task<IEnumerable<HistoricoReinicioSenhaDto>> ObterHistoricoReinicioSenhaUsuarioPorDre(string CodigoDre);
    }
}
