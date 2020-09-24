using SME.SR.Infra;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public interface IRecuperacaoParalelaRepository
    {
        Task<RecuperacaoParalelaPeriodo> ObterPeriodoPorId(long id);

        Task<IEnumerable<RetornoResumoPAPTotalAlunosAnoDto>> ListarTotalAlunosSeries(int? periodoId, string dreId, string ueId, int? cicloId, string turmaId, string ano, int anoLetivo);

        Task<IEnumerable<RetornoResumoPAPTotalAlunosAnoFrequenciaDto>> ListarTotalEstudantesPorFrequencia(int? periodoId, string dreId, string ueId, int? cicloId, string turmaId, string ano, int anoLetivo);

        Task<IEnumerable<RetornoResumoPAPTotalResultadoDto>> ListarTotalResultado(int? periodoId, string dreId, string ueId, int? cicloId, string turmaId, string ano, int anoLetivo, int? pagina);

        Task<IEnumerable<RetornoResumoPAPTotalResultadoDto>> ListarTotalResultadoEncaminhamento(int? periodoId, string dreId, string ueId, int? cicloId, string turmaId, string ano, int anoLetivo, int? pagina);
    }
}
