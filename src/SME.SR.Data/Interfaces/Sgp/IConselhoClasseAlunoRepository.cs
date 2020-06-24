using SME.SR.Infra;
using System.Threading.Tasks;

namespace SME.SR.Data.Interfaces
{
    public interface IConselhoClasseAlunoRepository
    {
        Task<string> ObterParecerConclusivo(long conselhoClasseId, string codigoAluno);

        Task<RecomendacaoConselhoClasseAluno> ObterRecomendacoesPorFechamento(long fechamentoTurmaId, string codigoAluno);

        Task<bool> PossuiConselhoClasseCadastrado(long conselhoClasseId, string codigoAluno);
    }
}
