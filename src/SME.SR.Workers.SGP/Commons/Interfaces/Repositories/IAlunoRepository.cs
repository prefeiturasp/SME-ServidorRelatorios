using SME.SR.Workers.SGP.Models;
using System.Threading.Tasks;

namespace SME.SR.Workers.SGP.Commons
{
    public interface IAlunoRepository
    {
        Task<Aluno> ObterDados(string codigoTurma, string codigoAluno);
    }
}
