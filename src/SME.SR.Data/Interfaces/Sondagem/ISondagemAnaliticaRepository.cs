using SME.SR.Data.Models;
using SME.SR.Infra;
using SME.SR.Infra.Dtos.Sondagem;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data.Interfaces
{
    public interface ISondagemAnaliticaRepository
    {
        Task<IEnumerable<RelatorioSondagemAnaliticoPorDreDto>> ObterRelatorioSondagemAnaliticoNumero(FiltroRelatorioAnaliticoSondagemDto filtro);
        Task<IEnumerable<RelatorioSondagemAnaliticoPorDreDto>> ObterRelatorioSondagemAnaliticoIAD(FiltroRelatorioAnaliticoSondagemDto filtro);
        Task<PeriodoFixoSondagem> ObterPeriodoFixoSondagem(string termoPeriodo, int anoLetivo);
        Task<IEnumerable<TotalRespostasAnaliticoLeituraDto>> ObterRespostasRelatorioAnaliticoDeLeitura(FiltroRelatorioAnaliticoSondagemDto filtro, bool valorSemPreenchimento);
        Task<IEnumerable<TotalRespostasAnaliticoEscritaDto>> ObterRespostaRelatorioAnaliticoDeEscrita(FiltroRelatorioAnaliticoSondagemDto filtro, bool valorSemPreenchimento);
    }
}
