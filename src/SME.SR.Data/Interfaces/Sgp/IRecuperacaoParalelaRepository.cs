using SME.SR.Infra;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public interface IRecuperacaoParalelaRepository
    {
        Task<RecuperacaoParalelaPeriodo> ObterPeriodoPorId(long id);

        Task<IEnumerable<ResumoPAPTotalAlunosAnoDto>> ListarTotalAlunosSeries(int? periodo, string dreId, string ueId, int? cicloId, string turmaId, string ano, int anoLetivo);
    }
}
