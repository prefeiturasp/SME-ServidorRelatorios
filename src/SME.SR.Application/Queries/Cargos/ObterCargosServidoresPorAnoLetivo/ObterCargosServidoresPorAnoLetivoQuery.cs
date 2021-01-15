using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterCargosServidoresPorAnoLetivoQuery : IRequest<IEnumerable<ServidorCargoDto>>
    {
        public ObterCargosServidoresPorAnoLetivoQuery(int anoLetivo, string[] codigosRF)
        {
            AnoLetivo = anoLetivo;
            CodigosRF = codigosRF;
        }

        public int AnoLetivo { get; set; }

        public string[] CodigosRF { get; set; }
    }
}
