using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data.Interfaces
{
    public interface ITurmaRepository
    {
        Task<DreUe> ObterDreUe(string codigoTurma);
        Task<DreUe> ObterDreUePorTurmaId(long turmaId);
        Task<TurmaResumoDto> ObterTurmaResumoComDreUePorId(long turmaId);
        Task<IEnumerable<Aluno>> ObterDadosAlunos(string codigoTurma);
        Task<Turma> ObterComDreUePorCodigo(string codigoTurma);

        Task<IEnumerable<Turma>> ObterTurmasPorAno(int anoLetivo, string[] anosEscolares);

        Task<IEnumerable<Turma>> ObterTurmasPorIds(long[] ids);
        Task<IEnumerable<Turma>> ObterTurmasPorCodigos(string[] codigos);

        Task<string> ObterCicloAprendizagem(string turmaCodigo);
        Task<IEnumerable<AlunoSituacaoDto>> ObterDadosAlunosSituacao(string turmaCodigo);        
        Task<Turma> ObterPorCodigo(string turmaCodigo);
        Task<Turma> ObterPorId(long turmaId);
        Task<IEnumerable<Turma>> ObterPorAbrangenciaFiltros(string codigoUe, Modalidade modalidade, int anoLetivo, string login, Guid perfil, bool consideraHistorico, int semestre, bool? possuiFechamento = null, bool? somenteEscolarizada = null, string codigoDre = null);
        Task<IEnumerable<Turma>> ObterPorAbrangenciaTiposFiltros(string codigoUe, string login, Guid perfil, Modalidade modalidade, int[] tipos, SituacaoFechamento? situacaoFechamento, SituacaoConselhoClasse? situacaoConselhoClasse, int[] bimestres, int semestre = 0, bool consideraHistorico = false, int anoLetivo = 0, bool? possuiFechamento = null, bool? somenteEscolarizada = null, string codigoDre = null);
        Task<IEnumerable<long>> ObterTurmasCodigoPorUeAnoSondagemAsync(string ano, string ueCodigo, int anoLetivo, long dreCodigo);
        Task<IEnumerable<TurmaFiltradaUeCicloAnoDto>> ObterPorUeCicloAno(int anoLetivo, string ano, long tipoCicloId, long ueId);
        Task<IEnumerable<AlunosTurmasCodigosDto>> ObterPorAlunosEParecerConclusivo(long[] codigoAlunos, long[] codigoPareceresConclusivos);
        Task<IEnumerable<AlunosTurmasCodigosDto>> ObterAlunosCodigosPorTurmaParecerConclusivo(long turmaCodigo, long[] codigoPareceresConclusivos);
        Task<IEnumerable<AlunosTurmasCodigosDto>> ObterPorAlunosSemParecerConclusivo(long[] codigoAlunos);
        Task<IEnumerable<AlunosTurmasCodigosDto>> ObterAlunosCodigosPorTurmaSemParecerConclusivo(long turmaCodigo);
        Task<IEnumerable<Turma>> ObterTurmasPorAnoEModalidade(int anoLetivo, string[] anosEscolares, Modalidade modalidade);
        Task<IEnumerable<AlunoDaTurmaDto>> ObterAlunosPorTurmas(IEnumerable<long> turmasCodigo);
        Task<IEnumerable<AlunoDaTurmaDto>> ObterAlunosPorTurmasAnosAnteriores(IEnumerable<long> turmasCodigo);
        Task<IEnumerable<TurmaFiltradaUeCicloAnoDto>> ObterTurmasPorUeAnosModalidadeESemestre(string[] uesCodigos, string[] anos, int modalidade, int? semestre);
        Task<IEnumerable<Aluno>> ObterDadosAlunosPorTurmaNotasConceitos(string codigoTurma);
        Task<IEnumerable<AlunoTurmaRegularRetornoDto>> ObterAlunosTurmasRegularesPorTurmaRecuperacaoCodigoQuery(long turmaCodigo);
        Task<IEnumerable<Turma>> ObterTurmasPorUeEAnoLetivo(string ueCodigo, long anoLetivo);
        Task<IEnumerable<TurmaResumoDto>> ObterTurmasResumoPorCodigos(string[] turmaCodigos);
        Task<IEnumerable<Turma>> ObterTurmasDetalhePorCodigos(long[] turmaCodigos);
        Task<Turma> ObterComDreUePorId(long turmaId);
        Task<IEnumerable<Turma>> ObterTurmasPorCodigosSituacaoConsolidado(string[] codigos, SituacaoFechamento? situacaoFechamento, SituacaoConselhoClasse? situacaoConselhoClasse, int[] bimestres);
        Task<IEnumerable<Aluno>> ObterDadosAlunosPorTurmaDataMatricula(string codigoTurma, DateTime dataMatricula);
        Task<IEnumerable<TurmaItinerarioEnsinoMedioDto>> ObterTurmasItinerarioEnsinoMedio();
        Task<IEnumerable<Turma>> ObterTurmasComplementaresPorAlunos(string[] alunosCodigos);
    }
}
