using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterCargosAtividadesPorRfQuery : IRequest<IEnumerable<ServidorCargoDto>>
    {
        public string[] CodigosRf { get; set; }

        public ObterCargosAtividadesPorRfQuery(string[] codigosRf)
        {
            CodigosRf = codigosRf;
        }
    }
}
