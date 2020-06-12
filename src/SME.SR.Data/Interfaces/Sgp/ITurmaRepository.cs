using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data.Interfaces
{
    public interface ITurmaRepository
    {
        Task<DreUe> ObterDreUe(string codigoTurma);

        Task<IEnumerable<Aluno>> ObterDadosAlunos(string codigoTurma);
    }
}
