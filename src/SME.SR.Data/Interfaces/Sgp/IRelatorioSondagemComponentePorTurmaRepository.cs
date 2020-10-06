using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public interface IRelatorioSondagemComponentePorTurmaRepository
    {
        public List<RelatorioSondagemComponentesPorTurmaOrdemDto> ObterOrdens();
        public List<RelatorioSondagemComponentesPorTurmaPlanilhaQueryDto> ObterPlanilhaLinhas(int dreId, int turmaId, int ano, int semestre);
    }
}
