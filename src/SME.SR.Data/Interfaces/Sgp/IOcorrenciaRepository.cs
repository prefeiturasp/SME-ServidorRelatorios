using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data.Interfaces
{
    public interface IOcorrenciaRepository
    {
        Task<IEnumerable<AcompanhamentoAprendizagemOcorrenciaDto>> ObterOcorenciasPorTurmaEAluno(long turmaId, long? alunoCodigo, DateTime dataInicio, DateTime dataFim);
        Task<IEnumerable<OcorrenciasPorCodigoTurmaDto>> ObterOcorrenciasCodigoETurma(long turmaId, long[] ocorrenciaIds);
        Task<IEnumerable<RelatorioListagemOcorrenciasRegistroDto>> ObterListagemOcorrenciasAsync(int anoLetivo, string codigoDre, string codigoUe, int modalidade, int semestre, string[] codigosTurma, DateTime? dataInicio, DateTime? dataFim, long[] ocorrenciaTipoIds);
        Task<IEnumerable<RelatorioListagemOcorrenciasRegistroAlunoDto>> ObterAlunosOcorrenciasPorIdsAsync(int[] ocorrenciaIds);
        Task<IEnumerable<RelatorioListagemOcorrenciasRegistroServidorDto>> ObterServidoresOcorrenciasPorIds(int[] ocorrenciaIds);
    }
}