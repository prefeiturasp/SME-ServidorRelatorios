using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public interface INotasFrequenciaAlunoBimestreRepository
    {
        Task<IEnumerable<NotasFrequenciaAlunoBimestre>> ObterNotasFrequenciaAlunosBimestre(string[] codigosTurma, string[] codigosAluno);
    }
}
