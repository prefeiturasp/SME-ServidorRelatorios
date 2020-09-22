using SME.SR.Data.Models;
using SME.SR.Infra;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public interface ICicloRepository
    {
        Task<long?> ObterCicloIdPorAnoModalidade(string ano, Modalidade modalidadeCodigo);

        Task<IEnumerable<TipoCiclo>> ObterCiclosPorAnosModalidade(string[] anos, Modalidade modalidadeCodigo);
        Task<IEnumerable<TipoCiclo>> ObterPorUeId(long ueId);
        Task<TipoCiclo> ObterPorId(long id);
    }
}
