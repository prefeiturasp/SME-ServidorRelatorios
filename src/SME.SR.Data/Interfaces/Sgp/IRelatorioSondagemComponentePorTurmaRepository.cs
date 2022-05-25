using SME.SR.Infra;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public interface IRelatorioSondagemComponentePorTurmaRepository
    {
        public Task<IEnumerable<RelatorioSondagemComponentesPorTurmaOrdemDto>> ObterOrdensAsync();

        public Task<IEnumerable<RelatorioSondagemComponentesPorTurmaPerguntasQueryDto>> ObterPerguntas(int anoLetivo, int anoTurma);

        public Task<IEnumerable<RelatorioSondagemComponentesPorTurmaPerguntasProficienciaQueryDto>> ObterPerguntasProficiencia(int anoLetivo, int anoTurma, ProficienciaSondagemEnum proficiencia);

        public Task<IEnumerable<RelatorioSondagemComponentesPorTurmaPlanilhaQueryDto>> ObterPlanilhaLinhas(string dreCodigo, string turmaCodigo, int anoLetivo,
            int semestre, ProficienciaSondagemEnum proficiencia, int anoTurma, string periodoId = "");

        public Task<IEnumerable<RelatorioSondagemComponentesPorTurmaPerguntasRespostasQueryDto>> ObterPerguntasRespostas(string dreCodigo, string turmaCodigo,
            int anoLetivo, int bimestre, int anoTurma, string componenteCurricularId, string periodoId = "");

        public Task<IEnumerable<RelatorioSondagemComponentesPorTurmaPerguntasRespostasProficienciaQueryDto>> ObterPerguntasRespostasProficiencia(string dreCodigo,
            string turmaCodigo, int anoLetivo, int bimestre, ProficienciaSondagemEnum proficiencia, int anoTurma, string componenteCurricularId, string periodoId = "");
    }
}
