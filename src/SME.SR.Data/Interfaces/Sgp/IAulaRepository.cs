using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data.Interfaces
{
    public interface IAulaRepository
    {
        Task<int> ObterAulasDadas(string codigoTurma, string componenteCurricularCodigo, long tipoCalendarioId, int bimestre);
        Task<AulaPrevista> ObterAulaPrevistaFiltro(long tipoCalendarioId, string turmaId, string disciplinaId);        
        Task<bool> VerificaExisteAulaCadastradaProfessorRegencia(string componenteCurricularId, int bimestre, long tipoCalendarioId);
        Task<bool> VerificaExisteAulaCadastrada(long turmaId, string componenteCurricularId, int bimestre, long tipoCalendarioId);
        Task<bool> VerificaExisteMaisAulaCadastradaNoDia(long turmaId, string componenteCurricularId, long tipoCalendarioId, int bimestre);
        Task<bool> VerificaExsiteAulaTitularECj(long turmaId, long componenteCurricularId, long tipoCalendarioId, int bimestre);
        Task<bool> VerificaExisteAulasMesmoDiaProfessor(long turmaId, long componenteCurricularId, long tipoCalendarioId, int bimestre);
    }
}
