using SME.SR.Infra;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data.Interfaces
{
    public interface IEncaminhamentoNAAPARepository
    {
        Task<IEnumerable<EncaminhamentoNAAPASimplesDto>> ObterResumoEncaminhamentosNAAPA(FiltroRelatorioEncaminhamentoNAAPADto filtro);
    }
}
