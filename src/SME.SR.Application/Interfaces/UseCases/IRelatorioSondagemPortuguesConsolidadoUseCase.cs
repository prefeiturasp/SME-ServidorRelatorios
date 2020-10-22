using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public interface IRelatorioSondagemPortuguesConsolidadoUseCase
    {
        Task<string> Executar(FiltroRelatorioSincronoDto request);
    }
}
