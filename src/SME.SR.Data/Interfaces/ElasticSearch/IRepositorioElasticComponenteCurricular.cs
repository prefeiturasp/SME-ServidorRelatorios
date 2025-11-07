using SME.SR.Data.Interfaces.ElasticSearch.Base;
using SME.SR.Infra.Dtos.ElasticSearch;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data.Interfaces.ElasticSearch
{
    public interface IRepositorioElasticComponenteCurricular : IRepositorioElasticBase<DocumentoElasticTurma>
    {
        Task<IEnumerable<ComponenteCurricular>> ObterComponentesCurricularesAsync(string[] codigosTurmas);
    }
}