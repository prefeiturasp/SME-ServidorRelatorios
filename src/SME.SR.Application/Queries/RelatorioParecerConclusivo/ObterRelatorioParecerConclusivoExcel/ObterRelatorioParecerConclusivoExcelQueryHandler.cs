using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterRelatorioParecerConclusivoExcelQueryHandler : IRequestHandler<ObterRelatorioParecerConclusivoExcelQuery, IEnumerable<RelatorioParecerConclusivoExcelDto>>
    {
        public async Task<IEnumerable<RelatorioParecerConclusivoExcelDto>> Handle(ObterRelatorioParecerConclusivoExcelQuery request, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}
