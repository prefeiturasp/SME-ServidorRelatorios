using SME.SR.Infra;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public interface IPeriodoSondagemRepository
    {
        Task<PeriodoCompletoSondagemDto> ObterPeriodoFixoFimPorDescricaoAnoLetivo(string descricao, int anoLetivo);
        Task<PeriodoCompletoSondagemDto> ObterPeriodoAberturaFimPorBimestreAnoLetivo(int bimestre, int anoLetivo);
        Task<PeriodoSondagem> ObterPeriodoPorTipo(int periodo, TipoPeriodoSondagem tipoPeriodo);
        Task<PeriodoCompletoSondagemDto> ObterPeriodoCompletoPorBimestreEAnoLetivo(int bimestre, string anoLetivo);
        Task<PeriodoCompletoSondagemDto> ObterPeriodoCompletoPorSemestreEAnoLetivo(int bimestre, string anoLetivo);
        Task<PeriodoCompletoSondagemDto> ObterPeriodoFixoCompletoPorDescricaoEAnoLetivo(string likeDescricao, int anoLetivo);
    }
}
