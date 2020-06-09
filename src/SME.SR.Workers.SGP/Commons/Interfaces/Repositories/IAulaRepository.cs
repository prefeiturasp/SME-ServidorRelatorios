using System.Threading.Tasks;

namespace SME.SR.Workers.SGP.Commons
{
    public interface IAulaRepository
    {
        Task<int> ObterAulasDadas(string codigoTurma, string disciplinaId, long tipoCalendarioId, int bimestre);
    }
}
