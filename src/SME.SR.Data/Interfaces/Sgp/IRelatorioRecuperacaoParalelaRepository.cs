using SME.SR.Infra;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data.Interfaces
{
    public interface IRelatorioRecuperacaoParalelaRepository
    {
        Task<IEnumerable<RelatorioRecuperacaoParalelaRetornoQueryDto>> ObterDadosDeSecao(string turmaCodigo, string alunoCodigo, int semestre);
    }
}
