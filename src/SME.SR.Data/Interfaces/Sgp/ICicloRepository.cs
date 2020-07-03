using SME.SR.Data.Models;
using SME.SR.Infra;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public interface ICicloRepository
    {
        Task<long?> ObterCicloIdPorAnoModalidade(int ano, Modalidade modalidadeCodigo);

        Task<IEnumerable<TipoCiclo>> ObterCiclosIdPorAnosModalidade(string[] anos, Modalidade modalidadeCodigo);
    }
}
