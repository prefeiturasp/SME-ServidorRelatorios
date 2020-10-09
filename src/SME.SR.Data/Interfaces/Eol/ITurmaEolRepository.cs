using System.Threading.Tasks;

namespace SME.SR.Data
{
    public interface ITurmaEolRepository
    {
        Task<Turma> ObterTurmaSondagemPorCodigo(long turmaCodigo);
    }
}
