using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public interface IRelatorioSondagemComponentePorTurmaRepository
    {
        public Task<IEnumerable<RelatorioSondagemComponentesPorTurmaOrdemDto>> ObterOrdensAsync();
        public Task<IEnumerable<RelatorioSondagemComponentesPorTurmaPlanilhaQueryDto>> ObterPlanilhaLinhas(string dreCodigo, string turmaCodigo, int ano, int semestre);
    }
}
