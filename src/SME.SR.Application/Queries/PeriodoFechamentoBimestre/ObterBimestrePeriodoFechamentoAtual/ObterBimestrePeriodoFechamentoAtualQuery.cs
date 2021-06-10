using MediatR;
using SME.SR.Data;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterBimestrePeriodoFechamentoAtualQuery : IRequest<int>
    {
        public ObterBimestrePeriodoFechamentoAtualQuery(long dreId, long ueId, int anoLetivo)
        {
            DreId = dreId;
            UeId = ueId;
            AnoLetivo = anoLetivo;
        }

        public long DreId { get; set; }
        public long UeId { get; set; }
        public int AnoLetivo { get; set; }
    }
}
