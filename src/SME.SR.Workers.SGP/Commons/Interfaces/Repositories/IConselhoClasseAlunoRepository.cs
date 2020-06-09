using SME.SR.Workers.SGP.Models;
using System.Threading.Tasks;

namespace SME.SR.Workers.SGP.Commons
{
    public interface IConselhoClasseAlunoRepository
    {
        Task<string> ObterParecerConclusivo(long conselhoClasseId, string codigoAluno);

        Task<RecomendacaoConselhoClasseAluno> ObterRecomendacoesPorFechamento(long fechamentoTurmaId, string codigoAluno);
    }
}
