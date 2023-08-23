using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public interface ITurmaEolRepository
    {
        Task<Turma> ObterTurmaSondagemPorCodigo(long turmaCodigo);
        Task<IEnumerable<int>> BuscarCodigosTurmasAlunoPorAnoLetivoAlunoAsync(int anoLetivo, string[] codigoAlunos, IEnumerable<int> tiposTurma, bool consideraHistorico = false, DateTime? dataReferencia = null, string ueCodigo = null);
        Task<IEnumerable<CodigosTurmasAlunoPorAnoLetivoAlunoEdFisicaDto>> BuscarCodigosTurmasAlunosPorAnoLetivoAluno(int anoLetivo, string[] codigoAlunos, IEnumerable<int> tiposTurma, bool consideraHistorico = false, DateTime? dataReferencia = null, string ueCodigo = null);
    }
}
