using SME.SR.Infra;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data.Interfaces
{
    public interface ITurmaRepository
    {
        Task<DreUe> ObterDreUe(string codigoTurma);

        Task<IEnumerable<Aluno>> ObterDadosAlunos(string codigoTurma);

        Task<IEnumerable<Turma>> ObterPorUe(string codigoUe, Modalidade? modalidade, int? anoLetivo, long? periodoEscolarId);

        Task<Turma> ObterPorCodigo(string codigoTurma);
    }
}
