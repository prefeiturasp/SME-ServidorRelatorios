using SME.SR.Infra;
using System;
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

        Task<IEnumerable<InformacaoPedagogicaComponenteCurricularSGPDTO>> ListarInformacoesPedagogicasComponentesCurriculares();

        Task<IEnumerable<ComponenteCurricularRegenciaApiEol>> ListarRegencia();

        Task<IEnumerable<ComponenteCurricularGrupoMatriz>> ListarGruposMatriz();

        Task<IEnumerable<ComponenteCurricularTerritorioSaber>> ObterComponentesTerritorioDosSaberes(string turmaCodigo, IEnumerable<long> componentesCurricularesId);
        Task<IEnumerable<ComponenteCurricularTerritorioSaber>> ObterComponentesTerritorioDosSaberes(string[] turmasCodigo, IEnumerable<long> componentesCurricularesId);
        Task<IEnumerable<ComponenteCurricular>> ObterComponentesPorAlunos(int[] codigosTurmas, int[] alunosCodigos, int anoLetivo, int semestre, bool consideraHistorico = false);
        Task<IEnumerable<ComponenteCurricular>> ObterComponentesPorCodigoETurma(string turmaCodigo, long[] componentesCodigo);

        Task<IEnumerable<ComponenteCurricularSondagem>> ObterComponenteCurricularDeSondagemPorId(string componenteCurricularId);
        Task<string> ObterNomeComponenteCurricularPorId(long componenteCurricularId);
        Task<long> ObterGrupoMatrizIdPorComponenteCurricularId(long componenteCurricularId);

        Task<bool> VerificaSeComponenteEhTerritorio(long componenteCurricularId);
        Task<IEnumerable<AgrupamentoAtribuicaoTerritorioSaber>> ObterAgrupamentosTerritorioSaber(long[] ids);
        Task<IEnumerable<AgrupamentoAtribuicaoTerritorioSaber>> ObterAgrupamentosTerritorioSaber(long codigoTurma,
                                                                                                 long? codigoTerritorioSaber,
                                                                                                 long? codigoExperienciaPegagogica,
                                                                                                 long? codigoComponenteCurricular = null,
                                                                                                 DateTime? dataBase = null,
                                                                                                 DateTime? dataInicio = null,
                                                                                                 string rfProfessor = null,
                                                                                                 string codigosComponentesCurriculares = null,
                                                                                                 bool? encerramentoAtribuicaoViaAtualizacaoComponentesAgrupados = null);
        Task<IEnumerable<ComponenteCurricularTerritorioAtribuidoTurmaDTO>> ObterComponentesCurricularesTerritorioAtribuidos(long codigoTurma, string rfProf = null);
    }
}
