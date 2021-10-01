﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public interface IPeriodoFechamentoRepository
    {
        Task<PeriodoFechamentoBimestre> ObterPeriodoFechamentoTurmaAsync(long ueId, long dreId, int anoLetivo, int bimestre, long? periodoEscolarId);

        Task<IEnumerable<PeriodoFechamentoBimestre>> ObterPeriodosFechamento(long ueId, long dreId, int anoLetivo);

        Task<int> ObterBimestrePeriodoFechamentoAtual(long ueId, long dreId, int anoLetivo);
    }
}
