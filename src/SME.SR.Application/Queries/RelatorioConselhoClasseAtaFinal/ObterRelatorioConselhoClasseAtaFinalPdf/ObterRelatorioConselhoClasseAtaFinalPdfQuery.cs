using MediatR;
using SME.SR.Infra;
using System;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterRelatorioConselhoClasseAtaFinalPdfQuery : IRequest<List<ConselhoClasseAtaFinalPaginaDto>>
    {
        public FiltroConselhoClasseAtaFinalDto FiltroConselhoClasseAtaFinal { get; set; }

        public string UsuarioLogadoRF { get; set; }

        public string PerfilUsuario { get; set; }

        public ObterRelatorioConselhoClasseAtaFinalPdfQuery(FiltroConselhoClasseAtaFinalDto filtroConselhoClasseAtaFinal,
                                                            string usuarioLogadoRF, string perfilUsuario)
        {
            this.FiltroConselhoClasseAtaFinal = filtroConselhoClasseAtaFinal;
            this.UsuarioLogadoRF = usuarioLogadoRF;
            this.PerfilUsuario = perfilUsuario;
        }
    }
}
