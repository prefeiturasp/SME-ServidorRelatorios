using SME.SR.Infra;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public interface IObterEnderecoeAtosDaUeRepository
    {
        Task<IEnumerable<EnderecoEAtosDaUeDto>> ObterEnderecoEAtos(string ueCodigo);
    }
}