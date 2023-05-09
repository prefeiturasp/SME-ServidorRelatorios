using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterListagemItineranciasQuery : IRequest<IEnumerable<ListagemItineranciaDto>>
    {
        public ObterListagemItineranciasQuery(FiltroRelatorioListagemItineranciasDto filtro)
        {
            this.filtro = filtro;
        }

        public FiltroRelatorioListagemItineranciasDto filtro { get; }
    }
}
