using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data.Interfaces
{
    public interface IAlunoRepository
    {
        Task<Aluno> ObterDados(string codigoTurma, string codigoAluno);

        Task<IEnumerable<AlunoHistoricoEscolar>> ObterDadosAlunoHistoricoEscolar(string[] codigosAlunos);
    }
}
