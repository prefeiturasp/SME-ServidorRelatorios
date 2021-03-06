﻿using SME.SR.Infra;
using System;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public interface IPeriodoSondagemRepository
    {
        Task<DateTime> ObterPeriodoFixoFimPorSemestreAnoLetivo(string semestreDescricao, int anoLetivo);
        Task<DateTime> ObterPeriodoAberturaFimPorBimestreAnoLetivo(int bimestre, int anoLetivo);
        Task<PeriodoSondagem> ObterPeriodoPorTipo(int periodo, TipoPeriodoSondagem tipoPeriodo);
    }
}
