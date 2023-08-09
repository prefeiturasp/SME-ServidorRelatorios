using SME.SR.Infra;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public interface IPerguntasAutoralRepository
    {
        Task<IEnumerable<PerguntasAutoralDto>> ObterPerguntasPorComponenteAnoTurma(int anoTurma, int anoLetivo, int bimestre, ComponenteCurricularSondagemEnum? componenteCurricularSondagem);

        Task<IEnumerable<PerguntasOrdemGrupoAutoralDto>> ObterPerguntasPorGrupo(GrupoSondagemEnum grupoSondagem, ComponenteCurricularSondagemEnum componenteCurricular);
    }
}
