using SME.SR.Infra;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data.Interfaces
{
    public interface IRegistroFrequenciaRepository
    {
        Task<IEnumerable<TurmaComponenteQtdAulasDto>> ObterTotalAulasPorDisciplinaETurmaEBimestre(string[] turmasCodigo, string[] componentesCurricularesId, long tipoCalendarioId, int[] bimestres);
        Task<IEnumerable<TurmaComponenteDataAulaQuantidadeDto>> ObterAulasPorTurmasComponentesTipoCalendarioBimestres(string[] turmasCodigo, string[] componentesCurricularesId, long tipoCalendarioId, int[] bimestres);
    }
}