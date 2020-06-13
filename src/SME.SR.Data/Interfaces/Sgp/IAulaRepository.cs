using System.Threading.Tasks;

namespace SME.SR.Data.Interfaces
{
    public interface IAulaRepository
    {
        Task<int> ObterAulasDadas(string codigoTurma, string disciplinaId, long tipoCalendarioId, int bimestre);
    }
}
