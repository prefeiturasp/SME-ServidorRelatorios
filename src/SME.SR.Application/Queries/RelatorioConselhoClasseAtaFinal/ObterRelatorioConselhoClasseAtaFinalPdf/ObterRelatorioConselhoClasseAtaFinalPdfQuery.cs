using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterRelatorioConselhoClasseAtaFinalPdfQuery : IRequest<List<ConselhoClasseAtaFinalPaginaDto>>
    {
        public FiltroConselhoClasseAtaFinalDto Filtro { get; set; }


        public ObterRelatorioConselhoClasseAtaFinalPdfQuery(FiltroConselhoClasseAtaFinalDto filtros)
        {
            this.Filtro = filtros;
        }
    }
}
