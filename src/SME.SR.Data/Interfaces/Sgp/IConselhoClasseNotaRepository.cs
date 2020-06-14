using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data.Interfaces
{
    public interface IConselhoClasseNotaRepository
    {
        Task<IEnumerable<NotaConceitoBimestreComponente>> ObterNotasAluno(long conselhoClasseId, string codigoAluno);

        Task<IEnumerable<NotaConceitoBimestreComponente>> ObterNotasFinaisAlunoBimestre(string codigoTurma, string codigoAluno);
    }
}
