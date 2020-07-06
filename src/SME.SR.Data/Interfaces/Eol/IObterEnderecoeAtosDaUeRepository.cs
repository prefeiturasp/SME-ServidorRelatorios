using SME.SR.Infra;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public interface IObterEnderecoeAtosDaUeRepository
    {
        Task<CabecalhoDto> ObterEnderecoEAtos(string ueCodigo);
    }
}
