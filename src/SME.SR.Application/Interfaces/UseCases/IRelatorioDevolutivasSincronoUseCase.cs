using SME.SR.Infra.Dtos;
using System;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public interface IRelatorioDevolutivasSincronoUseCase
    {
        Task<Guid> GerarRelatorioSincrono(FiltroRelatorioDevolutivasSincronoDto dto);
    }
}
