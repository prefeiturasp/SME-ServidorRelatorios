using SME.SR.Infra;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public interface IConselhoClasseAlunoTurmaComplementarRepository
    {
        Task<IEnumerable<TurmaComplementarConselhoClasseAluno>> ObterTurmasPorConselhoClasseAlunoIds(long[] conselhoClasseAlunoIds);
    }
}
