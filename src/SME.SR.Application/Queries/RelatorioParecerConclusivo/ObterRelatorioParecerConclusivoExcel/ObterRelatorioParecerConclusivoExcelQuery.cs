using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterRelatorioParecerConclusivoExcelQuery : IRequest<IEnumerable<RelatorioParecerConclusivoExcelDto>>
    {
        public RelatorioParecerConclusivoDto RelatorioParecerConclusivo { get; set; }
    }
}
