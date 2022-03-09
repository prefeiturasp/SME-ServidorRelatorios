using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public interface IPerguntasAditMultiNumRepository
    {
        Task<IEnumerable<PerguntasAditMultNumDto>> ObterPerguntasOrdem(int? anoTurma, int? anoLetivo, ComponenteCurricularSondagemEnum? componenteCurricularSondagem, ProficienciaSondagemEnum proficiencia);
        Task<IEnumerable<SondagemAutoralDto>> ObterPorFiltros(string codigoDre, string codigoUe, string grupoId, string periodoId, int bimestre, int? anoTurma, int? anoLetivo, ComponenteCurricularSondagemEnum? componenteCurricularSondagem);
        Task<IEnumerable<RespostaDescricaoDto>> ObterRespostasDaPergunta(string perguntaId);
    }
}
