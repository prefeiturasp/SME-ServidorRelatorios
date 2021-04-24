using SME.SR.Infra;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data.Interfaces
{
    public interface IOcorrenciaRepository
    {
        Task<IEnumerable<AcompanhamentoAprendizagemOcorrenciaDto>> ObterOcorenciasPorTurmaEAluno(long turmaId, long? alunoCodigo);
    }
}