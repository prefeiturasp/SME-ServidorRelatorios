using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public interface IRelatorioSondagemPortuguesPorTurmaRepository
    {
        public Task<IEnumerable<RelatorioSondagemPortuguesPorTurmaPlanilhaLinhaDto>> ObterPlanilhaLinhas(string dreCodigo, long turmaCodigo, int ano, int semestre, ProficienciaSondagemEnum proficiencia);
    }
}
