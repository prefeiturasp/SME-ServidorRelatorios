using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.Data.Interfaces
{
    public interface IPlanoAulaRepository
    {
        Task<PlanoAulaDto> ObterPorId(long planoAulaId);

        Task<IEnumerable<ObjetivoAprendizagemDto>> ObterObjetivoAprendizagemPorPlanoAulaId(long planoAulaId);
        Task<DateTime?> ObterUltimoPlanoAulaProfessor(string professorRf);
    }
}

