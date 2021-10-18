using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public interface IRegistrosPedagogicosRepository
    {
        Task<IEnumerable<ConsolidacaoRegistrosPedagogicosDto>> ObterDadosConsolidacaoRegistrosPedagogicos(int AnoLetivo, long[] componentesCurriculares, long[] TurmasId, string professorCodigo, string professorNome, List<int> bimestres);
        Task<IEnumerable<ConsolidacaoRegistrosPedagogicosDto>> ObterDadosConsolidacaoRegistrosPedagogicosInfantil(int AnoLetivo, long[] TurmasId, string professorCodigo, string professorNome, List<int> bimestres);
    }
}
