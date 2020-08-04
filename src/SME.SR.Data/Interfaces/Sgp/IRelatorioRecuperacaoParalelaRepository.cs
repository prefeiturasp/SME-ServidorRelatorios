using SME.SR.Infra;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data.Interfaces
{
    public interface IRelatorioRecuperacaoParalelaRepository
    {
        Task<IEnumerable<RelatorioRecuperacaoParalelaAlunoDto>> Obter(int idRecuperacaoParalela, long turmaId, int semestre);
    }
}
