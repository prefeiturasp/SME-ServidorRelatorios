using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public interface IRelatorioSondagemComponentePorTurmaRepository
    {
        Task<IEnumerable<RelatorioSondagemComponentesPorTurmaRetornoQueryDto>> ObterRelatorio(int dreId, int turmaId, int ueId, int ano);
    }
}
