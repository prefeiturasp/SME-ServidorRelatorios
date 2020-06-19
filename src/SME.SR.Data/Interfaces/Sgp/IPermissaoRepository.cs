using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data.Interfaces
{
    public interface IPermissaoRepository
    {
       Task<IEnumerable<GrupoAbrangenciaApiEol>> ObterTodosGrupos();
    }
}
