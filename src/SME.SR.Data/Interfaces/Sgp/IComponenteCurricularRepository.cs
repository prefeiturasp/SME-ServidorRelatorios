using SME.SR.Infra;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data.Interfaces
{
    public interface IComponenteCurricularRepository
    {
        Task<IEnumerable<ComponenteCurricular>> ObterComponentesPorTurmaEProfessor(string login, string codigoTurma);

        Task<IEnumerable<ComponenteCurricular>> ObterComponentesCurricularesPorTurmas(string[] codigosTurma);
        Task<IEnumerable<ComponenteCurricular>> ObterComponentesPorTurma(string codigoTurma);

        Task<IEnumerable<ComponenteCurricular>> ObterComponentesPorTurmas(string[] codigosTurma);

        Task<IEnumerable<ComponenteCurricular>> ListarComponentes();

        Task<IEnumerable<ComponenteCurricular>> ListarComponentesTerritorioSaber(string[] ids, string[] turmasId);

        Task<IEnumerable<ComponenteCurricularApiEol>> ListarApiEol();

        Task<IEnumerable<ComponenteCurricularRegenciaApiEol>> ListarRegencia();

        Task<IEnumerable<ComponenteCurricularGrupoMatriz>> ListarGruposMatriz();

        Task<IEnumerable<ComponenteCurricularTerritorioSaber>> ObterComponentesTerritorioDosSaberes(string turmaCodigo, IEnumerable<long> componentesCurricularesId);
        Task<IEnumerable<ComponenteCurricularTerritorioSaber>> ObterComponentesTerritorioDosSaberes(string[] turmasCodigo, IEnumerable<long> componentesCurricularesId);
        Task<IEnumerable<ComponenteCurricular>> ObterComponentesPorAlunos(int[] codigosTurmas, int[] alunosCodigos, int anoLetivo, int semestre, bool consideraHistorico = false);
        Task<IEnumerable<ComponenteCurricular>> ObterComponentesPorCodigoETurma(string turmaCodigo, long[] componentesCodigo);

        Task<IEnumerable<ComponenteCurricularSondagem>> ObterComponenteCurricularDeSondagemPorId(string componenteCurricularId);
        Task<string> ObterNomeComponenteCurricularPorId(long componenteCurricularId);

        Task<IEnumerable<DisciplinaDto>> ObterDisciplinasPorIds(long[] ids);

        Task<long> ObterGrupoMatrizIdPorComponenteCurricularId(long componenteCurricularId);

        Task<bool> VerificaSeComponenteEhTerritorio(long componenteCurricularId);
        Task<IEnumerable<ComponenteCurricularApiEol>> ObterComponentesCurricularesAPIEol();
    }
}
