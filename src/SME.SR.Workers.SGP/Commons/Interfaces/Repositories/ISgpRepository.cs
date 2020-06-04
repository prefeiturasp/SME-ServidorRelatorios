using SME.SR.Workers.SGP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SME.SR.Workers.SGP.Commons.Interfaces.Repositories
{
    public interface IEolRepository
    {
        public Task<List<DadosAluno>> ObterDadosAlunos();
    }
}
