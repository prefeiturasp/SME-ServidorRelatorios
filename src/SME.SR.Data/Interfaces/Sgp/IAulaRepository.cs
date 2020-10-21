using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data.Interfaces
{
    public interface IAulaRepository
    {
        Task<int> ObterAulasDadas(string codigoTurma, string componenteCurricularCodigo, long tipoCalendarioId, int bimestre);
        Task<bool> VerificaExisteAulaCadastrada(long turmaId, string componenteCurricularId, int bimestre, long tipoCalendarioId);
        Task<bool> VerificaExisteMaisAulaCadastradaNoDia(long turmaId, string componenteCurricularId, long tipoCalendarioId, int bimestre);
    }
}
