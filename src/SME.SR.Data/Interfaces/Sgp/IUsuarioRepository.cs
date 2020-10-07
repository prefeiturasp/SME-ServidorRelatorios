using System.Threading.Tasks;

namespace SME.SR.Data
{
    public interface IUsuarioRepository
    {
        Task<Usuario> ObterPorCodigoRF(string codigoRf);
    }
}
