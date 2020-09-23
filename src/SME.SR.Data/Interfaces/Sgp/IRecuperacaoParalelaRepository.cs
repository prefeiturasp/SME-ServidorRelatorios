using SME.SR.Infra;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public interface IRecuperacaoParalelaRepository
    {
        Task<RecuperacaoParalelaPeriodo> ObterPeriodoPorId(long id);

        Task<IEnumerable<RetornoResumoPAPTotalAlunosAnoDto>> ListarTotalAlunosSeries(int? periodo, string dreId, string ueId, int? cicloId, string turmaId, string ano, int anoLetivo);

        Task<IEnumerable<RetornoResumoPAPTotalAlunosAnoFrequenciaDto>> ListarTotalEstudantesPorFrequencia(int? periodo, string dreId, string ueId, int? cicloId, string turmaId, string ano, int anoLetivo);

        Task<IEnumerable<RetornoResumoPAPTotalResultadoDto>> ListarTotalResultado(int? periodo, string dreId, string ueId, int? cicloId, string turmaId, string ano, int anoLetivo, int? pagina);

        Task<IEnumerable<RetornoResumoPAPTotalResultadoDto>> ListarTotalResultadoEncaminhamento(int? periodo, string dreId, string ueId, int? cicloId, string turmaId, string ano, int anoLetivo, int? pagina);
    }
}
