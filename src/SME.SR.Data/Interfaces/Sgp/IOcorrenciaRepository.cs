using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data.Interfaces
{
    public interface IOcorrenciaRepository
    {
        Task<IEnumerable<AcompanhamentoAprendizagemOcorrenciaDto>> ObterOcorenciasPorTurmaEAluno(long turmaId, long? alunoCodigo, DateTime dataInicio, DateTime dataFim);
        Task<IEnumerable<OcorrenciasPorCodigoTurmaDto>> ObterOcorrenciasCodigoETurma(string turmaId, string[] ocorenciaCodigo);
    }
}