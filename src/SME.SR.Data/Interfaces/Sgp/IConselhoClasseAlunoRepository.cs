using SME.SR.Infra;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data.Interfaces
{
    public interface IConselhoClasseAlunoRepository
    {
        Task<string> ObterParecerConclusivo(long conselhoClasseId, string codigoAluno);

        Task<IEnumerable<ConselhoClasseParecerConclusivo>> ObterParecerConclusivoPorTurma(string turmaCodigo);

        Task<RecomendacaoConselhoClasseAluno> ObterRecomendacoesPorFechamento(long fechamentoTurmaId, string codigoAluno);
    }
}
