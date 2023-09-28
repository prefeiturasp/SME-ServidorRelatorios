using SME.SR.Infra;
using System;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public interface IPeriodoSondagemRepository
    {
        Task<DateTime> ObterPeriodoFixoFimPorDescricaoAnoLetivo(string descricao, int anoLetivo);
        Task<DateTime> ObterPeriodoAberturaFimPorBimestreAnoLetivo(int bimestre, int anoLetivo);
        Task<PeriodoSondagem> ObterPeriodoPorTipo(int periodo, TipoPeriodoSondagem tipoPeriodo);
        Task<PeriodoCompletoSondagemDto> ObterPeriodoCompletoPorBimestreEAnoLetivo(int bimestre, string anoLetivo);
        Task<PeriodoCompletoSondagemDto> ObterPeriodoCompletoPorSemestreEAnoLetivo(int bimestre, string anoLetivo);
    }
}
