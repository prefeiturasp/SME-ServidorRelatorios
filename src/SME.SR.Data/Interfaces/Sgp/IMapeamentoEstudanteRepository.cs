﻿using SME.SR.Infra;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data.Interfaces
{
    public interface IMapeamentoEstudanteRepository
    {
        Task<IEnumerable<MapeamentoEstudanteUltimoBimestreDto>> ObterMapeamentosEstudantesFiltro(FiltroRelatorioMapeamentoEstudantesDto filtro);
    }
}
