using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<Usuario> ObterDados(string codigoRf);
    }
}
