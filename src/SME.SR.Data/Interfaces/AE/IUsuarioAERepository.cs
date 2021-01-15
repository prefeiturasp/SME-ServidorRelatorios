using SME.SR.Infra;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public interface IUsuarioAERepository
    {
        Task<IEnumerable<UsuarioAEDto>> ObterUsuarioAEPorCpfs(string[] cpfs);
    }
}
