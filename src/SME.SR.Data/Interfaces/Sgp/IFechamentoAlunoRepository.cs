using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data.Interfaces
{
    public interface IFechamentoAlunoRepository
    {
        Task<IEnumerable<FechamentoAlunoAnotacaoConselho>> ObterAnotacoesTurmaAlunoBimestreAsync(string codigoAluno, long fechamentoTurmaId);
    }
}
