using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data.Interfaces
{
    public interface IComponenteCurricularRepository
    {
        Task<IEnumerable<ComponenteCurricular>> ObterComponentesPorTurmaEProfessor(string login, string codigoTurma);

        Task<IEnumerable<ComponenteCurricular>> ObterComponentesPorTurma(string codigoTurma);

        Task<IEnumerable<ComponenteCurricular>> ListarComponentes();

        Task<IEnumerable<ComponenteCurricular>> ListarComponentesTerritorioSaber(string[] ids);

        Task<IEnumerable<ComponenteCurricularApiEol>> ListarApiEol();

        Task<IEnumerable<ComponenteCurricularRegenciaApiEol>> ListarRegencia();

        Task<IEnumerable<ComponenteCurricularGrupoMatriz>> ListarGruposMatriz();

        Task<IEnumerable<ComponenteCurricularTerritorioSaber>> ObterComponentesTerritorioDosSaberes(string turmaCodigo, IEnumerable<long> componentesCurricularesId);
    }
}
