using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
  public  class ObterRelatorioNotasEConceitosFinaisExcelQuery : IRequest<IEnumerable<RelatorioNotasEConceitosFinaisExcelDto>>
    {
        public RelatorioNotasEConceitosFinaisDto RelatorioNotasEConceitosFinais{ get; set; }
    }
}
