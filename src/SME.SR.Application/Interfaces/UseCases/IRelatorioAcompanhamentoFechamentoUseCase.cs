﻿using SME.SR.Application.Interfaces;
using SME.SR.Infra;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public interface IRelatorioAcompanhamentoFechamentoUseCase 
    {
        Task<RelatorioAcompanhamentoFechamentoPorUeDto> Executar(FiltroRelatorioDto filtro);
    }
}
