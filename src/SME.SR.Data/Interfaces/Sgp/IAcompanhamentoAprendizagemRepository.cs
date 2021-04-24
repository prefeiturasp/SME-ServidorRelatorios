using SME.SR.Infra;
using System.Threading.Tasks;

namespace SME.SR.Data.Interfaces
{
    public interface IAcompanhamentoAprendizagemRepository
    {
        Task<AcompanhamentoAprendizagemAlunoRetornoDto> ObterAcompanhamentoAprendizagemPorTurmaESemestre(long turmaId, string alunoCodigo, int semestre);
    }
}