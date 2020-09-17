using Dapper;
using Npgsql;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public class PlanoAulaRepository : IPlanoAulaRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public PlanoAulaRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<IEnumerable<PlanoAulaDto>> ObterPorId(long planoAulaId)
        {
            throw new NotImplementedException();
        }
    }
}
