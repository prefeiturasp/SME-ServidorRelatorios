using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public interface IRelatorioSondagemPortuguesPorTurmaRepository
    {
        public Task<IEnumerable<RelatorioSondagemPortuguesPorTurmaPlanilhaQueryDto>> ObterPlanilhaLinhas(string dreCodigo, string ueCodigo, string turmaCodigo, int anoLetivo, int anoTurma, int bimestre, ProficienciaSondagemEnum proficiencia, string nomeColunaBimestre, GrupoSondagemEnum grupo, int semestre);

        public Task<IEnumerable<SondagemAutoralPorAlunoDto>> ObterPorFiltros(string grupoId, string componenteCurricularId, string periodoId, int anoLetivo, string codigoTurma);
    }
}
