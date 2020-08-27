using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data.Interfaces
{
    public interface IAlunoRepository
    {
        Task<Aluno> ObterDados(string codigoTurma, string codigoAluno);

        Task<IEnumerable<Aluno>> ObterPorCodigosTurma(string[] codigosTurma);

        Task<IEnumerable<Aluno>> ObterPorCodigosAlunoETurma(string[] codigosTurma, string[] codigosAluno);

        Task<IEnumerable<AlunoHistoricoEscolar>> ObterDadosAlunosPorCodigos(long[] codigosAlunos);
    }
}
