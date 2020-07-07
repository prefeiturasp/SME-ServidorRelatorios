using MediatR;
using System;

namespace SME.SR.Application
{
    public class SalvarRelatorioJasperLocalCommand : IRequest<bool>
    {
        public SalvarRelatorioJasperLocalCommand(string jSessionId, Guid requestId, Guid exportID, Guid correlacaoId)
        {
            JSessionId = jSessionId;
            RequestId = requestId;
            ExportID = exportID;
            CorrelacaoId = correlacaoId;
        }
        public string JSessionId { get; set; }
        public Guid RequestId { get; set; }
        public Guid ExportID { get; set; }
        public Guid CorrelacaoId { get; set; }

    }
}
