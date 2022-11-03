using SME.SR.Infra;
using SME.SR.Infra.Dtos.Relatorios.Sondagem;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public interface ISondagemAutoralRepository
    {
        Task<IEnumerable<SondagemAutoralDto>> ObterPorFiltros(string codigoDre, string codigoUe, string grupoId, string periodoId, int bimestre, int? anoTurma, int? anoLetivo, ComponenteCurricularSondagemEnum? componenteCurricularSondagem);
        Task<IEnumerable<PerguntasRespostasDTO>> ObterSondagemPerguntaRespostaConsolidadoBimestre(string codigoDre, string codigoUe, int bimestre, int anoTurma, int anoLetivo, string componenteCurricularId);
    }
}
