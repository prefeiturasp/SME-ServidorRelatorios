using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public interface IRegistrosPedagogicosRepository
    {
        Task<IEnumerable<ConsolidacaoRegistrosPedagogicosDto>> ObterDadosConsolidacaoRegistrosPedagogicos(string dreCodigo, string ueCodigo, int AnoLetivo, long[] componentesCurriculares, List<string> turmasCodigo, string professorCodigo, string professorNome, List<int> bimestres, Modalidade modalidade);
        Task<IEnumerable<ConsolidacaoRegistrosPedagogicosDto>> ObterDadosConsolidacaoRegistrosPedagogicosInfantil(string dreCodigo, string ueCodigo, int AnoLetivo, string professorCodigo, string professorNome, List<int> bimestres, List<string> turmasCodigo = null);
    }
}
