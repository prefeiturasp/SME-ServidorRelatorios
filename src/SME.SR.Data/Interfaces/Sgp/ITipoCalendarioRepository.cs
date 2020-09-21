﻿using SME.SR.Infra;
using System.Threading.Tasks;

namespace SME.SR.Data.Interfaces
{
    public interface ITipoCalendarioRepository
    {
        Task<long> ObterPorAnoLetivoEModalidade(int anoLetivo, ModalidadeTipoCalendario modalidade, int semestre = 0);
        Task<TipoCalendarioDto> ObterPorId(long id);
    }
}
