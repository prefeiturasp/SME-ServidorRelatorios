using SME.SR.Workers.SGP.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Workers.SGP.Commons
{
    public interface IFechamentoAlunoRepository
    {
        Task<IEnumerable<FechamentoAlunoAnotacaoConselho>> ObterAnotacoesTurmaAlunoBimestreAsync(string codigoAluno, long fechamentoTurmaId);
    }
}
