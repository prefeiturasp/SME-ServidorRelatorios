using SME.SR.Infra.Dtos;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public interface IExemploRepository
    {
        Task<string> ObterGames();
        Task<UeDto> ObterUePorCodigo(string codigoUe);
    }
}
