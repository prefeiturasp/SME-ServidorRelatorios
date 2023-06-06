using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public interface IHistoricoEscolarObservacaoRepository
    {
        Task<IEnumerable<FiltroHistoricoEscolarAlunosDto>> ObterPorCodigosAlunosAsync(string[] codigosAlunos);
    }
}
