using SME.SR.Infra;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public interface IFechamentoConsolidadoRepository
    {
        Task<IEnumerable<FechamentoConsolidadoComponenteTurmaDto>> ObterFechamentoConsolidadoPorTurmas(string[] turmasCodigo);
    }
}
