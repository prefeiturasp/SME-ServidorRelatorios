using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data.Interfaces
{
    public interface IAulaRepository
    {
        Task<int> ObterAulasDadas(string codigoTurma, string componenteCurricularCodigo, long tipoCalendarioId, int bimestre);
        Task<bool> VerificaExisteAulaCadastrada(string turmaCodigo, string componenteCurricularId);
        Task<bool> VerificaExisteAulaCadastradaProfessorRegencia(long turmaId, string componenteCurricularId);
    }
}
