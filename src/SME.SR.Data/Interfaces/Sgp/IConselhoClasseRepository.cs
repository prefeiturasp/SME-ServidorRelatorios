using SME.SR.Infra;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data.Interfaces
{
    public interface IConselhoClasseRepository
    {
        Task<long> ObterConselhoPorFechamentoTurmaId(long fechamentoTurmaId);
        Task<IEnumerable<long>> ObterPareceresConclusivosPorTipoAprovacao(bool aprovado);
    }
}
