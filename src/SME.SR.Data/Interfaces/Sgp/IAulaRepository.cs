using System.Threading.Tasks;

namespace SME.SR.Data.Interfaces
{
    public interface IAulaRepository
    {
        Task<int> ObterAulasDadas(string codigoTurma, string componenteCurricularCodigo, long tipoCalendarioId, int bimestre);
    }
}
