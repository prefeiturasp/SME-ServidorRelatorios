using SME.SR.Infra;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public interface IFechamentoConsolidadoRepository
    {
        Task<IEnumerable<FechamentoConsolidadoComponenteTurmaDto>> ObterFechamentoConsolidadoPorTurmas(string[] turmasCodigo, int[] semestres, int[] bimestres);
        Task<IEnumerable<FechamentoConsolidadoTurmaDto>> ObterFechamentoConsolidadoPorTurmasTodasUe(string dreCodigo, int modalidade, int[] bimestres, SituacaoFechamento? situacao, int anoLetivo, int semestre, bool exibirHistorico);
    }
}
