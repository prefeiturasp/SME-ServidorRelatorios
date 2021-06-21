using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterRelatorioConselhoClasseAtaFinalPdfQuery : IRequest<List<ConselhoClasseAtaFinalPaginaDto>>
    {
        public FiltroConselhoClasseAtaFinalDto Filtro { get; set; }
        public Usuario Usuario { get; set; }


        public ObterRelatorioConselhoClasseAtaFinalPdfQuery(FiltroConselhoClasseAtaFinalDto filtros, Usuario usuario)
        {
            this.Filtro = filtros;
            this.Usuario = usuario;
        }
    }
}
