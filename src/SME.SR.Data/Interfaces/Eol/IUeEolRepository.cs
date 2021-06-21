using SME.SR.Infra;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public interface IUeEolRepository
    {
        Task<UeEolEnderecoDto> ObterEnderecoUePorCodigo(long ueCodigo);
    }
}