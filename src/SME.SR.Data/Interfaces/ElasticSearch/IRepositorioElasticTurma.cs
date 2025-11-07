using SME.SR.Data.Interfaces.ElasticSearch.Base;
using SME.SR.Infra;
using SME.SR.Infra.Dtos.ElasticSearch;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data.Interfaces.ElasticSearch
{
    public interface IRepositorioElasticTurma : IRepositorioElasticBase<DocumentoElasticTurma>
    {
        Task<IEnumerable<TurmaComponentesDto>> ObterListaTurmasAsync(string codigoUe, int[] tiposEscolaModalidade, long codigoTurma, int anoLetivo,
            bool ehProfessor, string codigoRf, bool consideraHistorico,DateTime periodoEscolarInicio, int modalidade);
        Task<IEnumerable<TurmaComponentesDto>> ObterTurmasAsync(int[] codigosTurmas);

        Task<IEnumerable<AlunoNaTurmaDTO>> ObterMatriculasAlunoNaTurma(int[] codigosTurmas);
        Task<IEnumerable<AlunoSituacaoDto>> ObterTodosAlunosNaTurmaAsync(long codigosTurma);
        Task<(DateTime? dataMatricula, DateTime? dataSituacao)> ObterMatriculasAlunoNaTurma(int codigoAluno, int codigoTurma);
    }
}