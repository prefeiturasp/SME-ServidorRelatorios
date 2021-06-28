using SME.SR.Infra;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public interface IConselhoClasseConsolidadoRepository
    {
        Task<IEnumerable<ConselhoClasseConsolidadoTurmaAlunoDto>> ObterConselhosClasseConsolidadoPorTurmasBimestreAsync(long[] turmasId, int bimestre, int situacaoConselhoClasse);
    }
}
