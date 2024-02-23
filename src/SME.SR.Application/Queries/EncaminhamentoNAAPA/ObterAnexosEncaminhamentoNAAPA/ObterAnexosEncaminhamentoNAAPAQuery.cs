using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterAnexosEncaminhamentoNAAPAQuery : IRequest<IEnumerable<ArquivoDto>>
    {
        public ObterAnexosEncaminhamentoNAAPAQuery(long encaminhamentoId, ImprimirAnexosNAAPA imprimirAnexosNAAPA)
        {
            EncaminhamentoId = encaminhamentoId;
            ImprimirAnexosNAAPA = imprimirAnexosNAAPA;
        }

        public long EncaminhamentoId { get; set; }
        public ImprimirAnexosNAAPA ImprimirAnexosNAAPA { get; set; }
    }
}
