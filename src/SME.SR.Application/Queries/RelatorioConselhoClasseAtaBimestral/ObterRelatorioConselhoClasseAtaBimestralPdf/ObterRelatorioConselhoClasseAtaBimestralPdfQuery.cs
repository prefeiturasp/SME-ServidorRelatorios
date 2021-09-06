using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterRelatorioConselhoClasseAtaBimestralPdfQuery : IRequest<List<ConselhoClasseAtaFinalPaginaDto>>
    {
        public FiltroConselhoClasseAtaBimestralDto Filtro { get; set; }


        public ObterRelatorioConselhoClasseAtaBimestralPdfQuery(FiltroConselhoClasseAtaBimestralDto filtros)
        {
            this.Filtro = filtros;
        }
    }
}
