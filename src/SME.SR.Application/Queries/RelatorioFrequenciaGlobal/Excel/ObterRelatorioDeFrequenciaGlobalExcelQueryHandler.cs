using MediatR;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application.Queries.RelatorioFrequenciaGlobal.Excel
{
    public class ObterRelatorioDeFrequenciaGlobalExcelQueryHandler : IRequestHandler<ObterRelatorioDeFrequenciaGlobalExcelQuery, IEnumerable<DataTable>>
    {
        public Task<IEnumerable<DataTable>> Handle(ObterRelatorioDeFrequenciaGlobalExcelQuery request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
