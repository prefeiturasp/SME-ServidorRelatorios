using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public interface INotaConceitoRepository
    {
        Task<IEnumerable<NotasAlunoBimestre>> ObterNotasTurmasAlunos(string[] codigosTurma, string[] codigosAluno);
    }
}
