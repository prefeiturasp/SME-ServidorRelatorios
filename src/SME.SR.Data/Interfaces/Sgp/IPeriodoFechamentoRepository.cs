using SME.SR.Data.Models;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public interface IPeriodoFechamentoRepository
    {
        Task<PeriodoFechamentoBimestre> ObterPeriodoFechamentoTurmaAsync(long ueId, long dreId, int anoLetivo, int bimestre, long? periodoEscolarId);

        Task<IEnumerable<PeriodoFechamentoBimestre>> ObterPeriodosFechamento(long ueId, long dreId, int anoLetivo);

        Task<int> ObterBimestrePeriodoFechamentoAtual(int anoLetivo);
        Task<PeriodoFechamentoVigenteDto> TurmaEmPeriodoDeFechamentoVigente(Turma turma, TipoCalendario tipoCalendario, DateTime dataReferencia, int bimestre = 0);
    }
}
