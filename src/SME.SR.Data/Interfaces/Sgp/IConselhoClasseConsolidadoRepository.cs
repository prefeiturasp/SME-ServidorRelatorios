using SME.SR.Infra;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public interface IConselhoClasseConsolidadoRepository
    {
        Task<IEnumerable<ConselhoClasseConsolidadoTurmaAlunoDto>> ObterConselhosClasseConsolidadoPorTurmasAsync(string[] turmasCodigo);
        Task<IEnumerable<ConselhoClasseConsolidadoTurmaAlunoDto>>  ObterConselhosClasseConsolidadoPorTurmasTodasUesAsync(string[] turmasCodigo)
    }
}
