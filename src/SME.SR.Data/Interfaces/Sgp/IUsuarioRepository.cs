using SME.SR.Infra;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<Usuario> ObterDados(string codigoRf);
        Task<IEnumerable<HistoricoReinicioSenhaDto>> ObterHistoricoReinicioSenhaUsuarioPorDre(string CodigoDre);
    }
}
