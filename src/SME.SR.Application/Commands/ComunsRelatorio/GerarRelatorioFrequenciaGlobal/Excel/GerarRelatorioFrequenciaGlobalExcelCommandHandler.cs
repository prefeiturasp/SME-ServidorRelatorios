using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application.Commands.ComunsRelatorio.GerarRelatorioFrequenciaGlobal.Excel
{
    public class GerarRelatorioFrequenciaGlobalExcelCommandHandler : IRequestHandler<GerarRelatorioFrequenciaGlobalExcelCommand, Unit>
    {
        public Task<Unit> Handle(GerarRelatorioFrequenciaGlobalExcelCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
