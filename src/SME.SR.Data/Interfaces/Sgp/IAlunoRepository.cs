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
        
        Task<IEnumerable<Aluno>> ObterAlunosPorTurmaDataSituacaoMaricula(long turmaCodigo, DateTime dataReferencia);
        Task<int> ObterTotalAlunosPorTurmasDataSituacaoMatriculaAsync(string anoTurma, string ueCodigo, int anoLetivo, long dreCodigo, DateTime dataReferencia, int[] modalidades);
        Task<IEnumerable<AlunoResponsavelAdesaoAEDto>> ObterAlunosResponsaveisPorTurmasCodigoParaRelatorioAdesao(long[] turmasCodigo, int anoLetivo);
        Task<IEnumerable<AlunoReduzidoDto>> ObterAlunosReduzidosPorTurmaEAluno(long turmaCodigo, long? alunoCodigo);
        Task<IEnumerable<AlunoRetornoDto>> ObterAlunosPorTurmaCodigoParaRelatorioAcompanhamentoAprendizagem(long turmaCodigo, long? alunoCodigo, int anoLetivo);
        Task<IEnumerable<AlunoPorTurmaRespostaDto>> ObterAlunosPorTurmaEDataMatriculaQuery(long turmaCodigo, DateTime dataReferencia);
        Task<IEnumerable<DadosAlunosEscolaDto>> ObterDadosAlunosEscola(string codigoEscola, int anoLetivo);
    }
}
