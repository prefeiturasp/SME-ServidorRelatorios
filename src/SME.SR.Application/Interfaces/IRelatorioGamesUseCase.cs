using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SME.SR.Workers.SGP
{
    public interface IRelatorioGamesUseCase
    {
        Task Executar(int ano);
    }
}
