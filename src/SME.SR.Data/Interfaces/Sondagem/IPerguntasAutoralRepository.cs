using SME.SR.Infra;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public interface IPerguntasAutoralRepository
    {
        Task<IEnumerable<PerguntasAutoralDto>> ObterPorFiltros(string codigoDre, string codigoUe, int? anoTurma, int? anoLetivo, ComponenteCurricularSondagemEnum? componenteCurricularSondagem);
    }
}
