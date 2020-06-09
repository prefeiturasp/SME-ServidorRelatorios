using SME.SR.Workers.SGP.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Workers.SGP.Commons
{
    public interface IEolRepository
    {
        Task<List<Aluno>> ObterDadosAlunos();
    }
}
