using SME.SR.Infra;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public interface IRelatorioSondagemPortuguesConsolidadoRepository
    {
        public Task<IEnumerable<RelatorioSondagemPortuguesConsolidadoLeituraPlanilhaQueryDto>> ObterPlanilha(string dreCodigo, string ueCodigo, string turmaCodigo, int anoLetivo, int anoTurma, GrupoSondagemEnum grupo, string periodoId);
    }
}
