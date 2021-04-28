using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data.Interfaces
{
    public interface IRegistroIndividualRepository
    {
        Task<IEnumerable<RegistroIndividualRetornoDto>> ObterRegistrosIndividuaisPorTurmaEAluno(long turmaId, long? alunoCodigo, DateTime dataInicio, DateTime dataFim);
    }
}