using SME.SR.Infra;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data.Interfaces
{
    public interface IConselhoClasseAlunoRepository
    {
        Task<string> ObterParecerConclusivo(long conselhoClasseId, string codigoAluno);

        Task<IEnumerable<ConselhoClasseParecerConclusivo>> ObterParecerConclusivoPorTurma(string turmaCodigo);

        Task<RecomendacoesConselhoClasse> ObterRecomendacoesPorFechamento(long fechamentoTurmaId, string codigoAluno);

        Task<IEnumerable<RecomendacaoConselhoClasseAluno>> ObterRecomendacoesPorAlunosTurmas(string[] codigosAluno, string[] codigosTurma, int anoLetivo, Modalidade? modalidade, int semestre);

        Task<bool> PossuiConselhoClasseCadastrado(long conselhoClasseId, string codigoAluno);
        Task<IEnumerable<AnotacoesPedagogicasAlunoIdsQueryDto>> ObterAnotacoesPedagogicasPorConselhoClasseAlunoIdsAsync(long[] conselhoClasseAlunoIds);

        Task<IEnumerable<RecomendacoesAlunoFamiliaDto>> ObterRecomendacoesAlunoFamiliaPorAlunoETurma(string codigoAluno, string codigoTurma, int id);
    }
}
