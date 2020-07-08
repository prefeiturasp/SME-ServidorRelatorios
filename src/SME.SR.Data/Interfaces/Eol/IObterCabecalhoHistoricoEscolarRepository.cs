using SME.SR.Infra;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public interface IObterCabecalhoHistoricoEscolarRepository
    {
        Task<CabecalhoDto> ObterCabecalhoHistoricoEscolar(int anoLetivo, string dreCodigo, string ueCodigo);
    }
}
