using SME.SR.Infra;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data.Interfaces
{
    public interface IPeriodoEscolarRepository
    {
        Task<IEnumerable<PeriodoEscolar>> ObterPeriodosEscolaresPorTipoCalendario(long tipoCalendarioId);
        Task<PeriodoEscolar> ObterUltimoPeriodoAsync(int anoLetivo, ModalidadeTipoCalendario modalidadeTipoCalendario, int semestre);
        Task<PeriodoEscolar> ObterPeriodoEscolarPorId(long idPeriodoEscolar);
    }
}
