using SME.SR.Infra;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public interface ISondagemOrdemRepository
    {
        Task<IEnumerable<SondagemOrdemDto>> ObterPorGrupo(GrupoSondagemEnum grupoSondagem);
    }
}
