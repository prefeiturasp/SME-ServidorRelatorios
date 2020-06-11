using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.Application.Interfaces
{
    public interface IUseCase
    {
        Task Executar(FiltroRelatorioDto request);
    }
}
