using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public interface ICicloRepository
    {
        Task<long?> ObterCicloIdPorAnoModalidade(int ano, Modalidade modalidadeCodigo);
    }
}
