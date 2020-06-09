using SME.SR.Workers.SGP.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Workers.SGP.Commons
{
    public interface IComponenteCurricularRepository
    {
        Task<IEnumerable<ComponenteCurricular>> ObterComponentesPorTurma(string codigoTurma);

        Task<IEnumerable<ComponenteCurricularApiEol>> Listar();

        Task<IEnumerable<ComponenteCurricularGrupoMatriz>> ListarGruposMatriz();
    }
}
