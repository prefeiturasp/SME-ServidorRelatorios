using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public interface INotaConceitoRepository
    {
        Task<IEnumerable<NotasAlunoBimestre>> ObterNotasTurmasAlunos(string[] codigosAluno, string[] codigosTurmas, int anoLetivo, int modalidade, int semestre);
        Task<IEnumerable<NotasAlunoBimestre>> ObterNotasTurmasAlunosParaHistoricoEscolasAsync(string[] codigosAluno, int anoLetivo, int modalidade, int semestre);
        Task<IEnumerable<NotasAlunoBimestre>> ObterNotasTurmasAlunosParaAtaFinalAsync(string[] codigosAlunos, int anoLetivo, int[] modalidade, int semestre, int[] tipoTurma);
        Task<IEnumerable<NotasAlunoBimestre>> ObterNotasTurmasAlunosParaAtaBimestralAsync(string[] codigosAlunos, int anoLetivo, int modalidade, int semestre, int[] tiposTurma, int bimestre);
    }
}