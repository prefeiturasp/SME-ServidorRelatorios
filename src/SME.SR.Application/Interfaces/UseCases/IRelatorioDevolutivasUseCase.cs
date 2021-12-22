using SME.SR.Infra;
using System;
using System.Threading.Tasks;

namespace SME.SR.Application.Interfaces
{
    public interface IRelatorioDevolutivasUseCase : IUseCase
    {
        Task<Guid> GerarRelatorioSincrono(int devolutivaId);
    }
}
