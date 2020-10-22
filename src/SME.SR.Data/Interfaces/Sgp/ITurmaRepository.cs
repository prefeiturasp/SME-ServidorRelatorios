using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data.Interfaces
{
    public interface ITurmaRepository
    {
        Task<DreUe> ObterDreUe(string codigoTurma);
        Task<TurmaResumoDto> ObterTurmaResumoComDreUePorId(long turmaId);
        Task<IEnumerable<Aluno>> ObterDadosAlunos(string codigoTurma);
        Task<Turma> ObterPorCodigo(string codigoTurma);

        Task<IEnumerable<Turma>> ObterTurmasPorAno(int anoLetivo, string[] anosEscolares);

        Task<string> ObterCicloAprendizagem(string turmaCodigo);
        Task<IEnumerable<AlunoSituacaoDto>> ObterDadosAlunosSituacao(string turmaCodigo);
        Task<IEnumerable<Turma>> ObterPorAbrangenciaFiltros(string codigoUe, Modalidade modalidade, int anoLetivo, string login, Guid perfil, bool consideraHistorico, int semestre, bool? possuiFechamento = null, bool? somenteEscolarizada = null);
        Task<IEnumerable<long>> ObterTurmasCodigoPorUeAnoSondagemAsync(string ano, string ueCodigo, int anoLetivo, long dreCodigo);
        Task<IEnumerable<TurmaFiltradaUeCicloAnoDto>> ObterPorUeCicloAno(string ano, long tipoCicloId, long ueId);
        Task<IEnumerable<AlunosTurmasCodigosDto>> ObterPorAlunosEParecerConclusivo(long[] codigoAlunos, long[] codigoPareceresConclusivos);
        Task<IEnumerable<AlunosTurmasCodigosDto>> ObterAlunosCodigosPorTurmaParecerConclusivo(long turmaCodigo, long[] codigoPareceresConclusivos);
        Task<IEnumerable<AlunosTurmasCodigosDto>> ObterPorAlunosSemParecerConclusivo(long[] codigoAlunos);
        Task<IEnumerable<AlunosTurmasCodigosDto>> ObterAlunosCodigosPorTurmaSemParecerConclusivo(long turmaCodigo);
        Task<IEnumerable<Turma>> ObterTurmasPorAnoEModalidade(int anoLetivo, string[] anosEscolares, Modalidade modalidade);
        Task<IEnumerable<Aluno>> ObterAlunosPorTurmas(IEnumerable<long> turmasCodigo);
        Task<IEnumerable<TurmaFiltradaUeCicloAnoDto>> ObterTurmasPorUeAnosModalidadeESemestre(string[] uesCodigos, string[] anos, int modalidade, int? semestre);
        Task<IEnumerable<Aluno>> ObterDadosAlunosPorTurmaNotasConceitos(string codigoTurma);
        Task<IEnumerable<AlunoTurmaRegularRetornoDto>> ObterAlunosTurmasRegularesPorTurmaRecuperacaoCodigoQuery(long turmaCodigo);
    }
}
