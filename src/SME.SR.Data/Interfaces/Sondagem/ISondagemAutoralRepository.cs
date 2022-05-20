using SME.SR.Infra;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public interface ISondagemAutoralRepository
    {
        Task<IEnumerable<SondagemAutoralDto>> ObterPorFiltros(string codigoDre, string codigoUe, string grupoId, string periodoId, int bimestre, int? anoTurma, int? anoLetivo, ComponenteCurricularSondagemEnum? componenteCurricularSondagem);
    }
}
