using SME.SR.Infra;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data.Interfaces
{
    public interface IRegistroIndividualRepository
    {
        Task<IEnumerable<AcompanhamentoAprendizagemRegistroIndividualDto>> ObterRegistroIndividualPorTurmaEAluno(long turmaId, long? alunoCodigo);
    }
}