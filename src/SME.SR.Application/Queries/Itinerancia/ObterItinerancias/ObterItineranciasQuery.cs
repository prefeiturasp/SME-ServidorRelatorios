using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterItineranciasQuery : IRequest<IEnumerable<RegistrosRegistroItineranciaDto>>
    {
        public ObterItineranciasQuery(IEnumerable<long> itinerancias)
        {
            Itinerancias = itinerancias;
        }

        public IEnumerable<long> Itinerancias { get; }
    }
}
