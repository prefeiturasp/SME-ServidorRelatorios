using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public interface IRelatorioSondagemPortuguesPorTurmaUseCase
    {
        Task<string> Executar(FiltroRelatorioSincronoDto request);
    }
}
