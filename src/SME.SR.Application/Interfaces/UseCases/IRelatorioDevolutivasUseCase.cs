using SME.SR.Infra;
using SME.SR.Infra.Dtos;
using System;
using System.Threading.Tasks;

namespace SME.SR.Application.Interfaces
{
    public interface IRelatorioDevolutivasUseCase : IUseCase
    {
        Task<Guid> GerarRelatorioSincrono(FiltroRelatorioDevolutivasSincronoDto dto);
    }
}
