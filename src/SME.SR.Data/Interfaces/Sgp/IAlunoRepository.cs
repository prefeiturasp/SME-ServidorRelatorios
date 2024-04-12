using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data.Interfaces
{
    public interface IAlunoRepository
    {
        Task<Aluno> ObterDados(string codigoTurma, string codigoAluno);

        Task<IEnumerable<Aluno>> ObterPorCodigosTurma(IEnumerable<string> codigosTurma);

        Task<IEnumerable<Aluno>> ObterPorCodigosAlunoETurma(string[] codigosTurma, string[] codigosAluno);

        Task<IEnumerable<AlunoHistoricoEscolar>> ObterDadosAlunosPorCodigos(long[] codigosAlunos, int? anoLetivo = null);

        Task<IEnumerable<AlunoHistoricoEscolar>> ObterDadosAlunosPorCodigosEAnoLetivo(long[] codigosAlunos, long anoLetivo);

        Task<IEnumerable<AlunoHistoricoEscolar>> ObterDadosHistoricoAlunosPorCodigos(long[] codigosAlunos);

        Task<IEnumerable<AlunoHistoricoEscolar>> ObterDadosAlunoHistoricoEscolar(long[] codigosAlunos);
        
        Task<AlunoNomeDto> ObterNomeAlunoPorCodigo(string codigos);
        
        Task<IEnumerable<AlunoNomeDto>> ObterNomesAlunosPorCodigos(string[] codigos);
        
        Task<IEnumerable<Aluno>> ObterAlunosPorTurmaDataSituacaoMatriculaParaSondagem(long turmaCodigo, DateTime dataReferenciaFim, DateTime? dataReferenciaInicio = null);
        Task<int> ObterTotalAlunosPorTurmasDataSituacaoMatriculaAsync(string anoTurma, string ueCodigo, int anoLetivo, long dreCodigo, DateTime dataReferenciaFim, int[] modalidades, bool consideraHistorico = false, DateTime? dataReferenciaInicio = null);
        Task<IEnumerable<AlunoResponsavelAdesaoAEDto>> ObterAlunosResponsaveisPorTurmasCodigoParaRelatorioAdesao(long[] turmasCodigo, int anoLetivo);
        Task<IEnumerable<AlunoReduzidoDto>> ObterAlunosReduzidosPorTurmaEAluno(long turmaCodigo, long? alunoCodigo);
        Task<IEnumerable<AlunoRetornoDto>> ObterAlunosPorTurmaCodigoParaRelatorioAcompanhamentoAprendizagem(long turmaCodigo, long? alunoCodigo, int anoLetivo);
        Task<IEnumerable<AlunoPorTurmaRespostaDto>> ObterAlunosPorTurmaEDataMatriculaQuery(long turmaCodigo, DateTime dataReferencia);
        Task<IEnumerable<DadosAlunosEscolaDto>> ObterDadosAlunosEscola(string ueCodigo, string dreCodigo, int anoLetivo, string[] codigosAlunos);
        Task<NecessidadeEspecialAlunoDto> ObterNecessidadesEspeciaisPorAluno(long codigoAluno);
        Task<(DateTime dataMatricula, DateTime dataSituacao)> ObterDatasMatriculaAlunoNaTurma(int codigoAluno, int codigoTurma);
        Task<IEnumerable<AlunoTurma>> ObterAlunosMatriculasPorTurmas(int[] codigosTurmas);
        Task<IEnumerable<TotalAlunosAnoTurmaDto>> ObterTotalAlunosAtivosPorPeriodoEAnoTurma(int anoLetivo, int[] modalidades, DateTime dataInicio, DateTime dataFim, string ueId, string dreId);
        Task<int> ObterTotalAlunosAtivosPorTurmaEPeriodo(string codigoTurma, DateTime dataInicio, DateTime dataReferencia);
    }
}
