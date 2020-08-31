using MediatR;
using SME.SR.Infra;
using System;

namespace SME.SR.Application
{
    public class ObterRelatorioImpressaoCalendarioQuery : IRequest<RelatorioImpressaoCalendarioDto>
    {
        public ObterRelatorioImpressaoCalendarioQuery(string dreCodigo, string ueCodigo, long tipoCalendarioId, bool ehSME, string usuarioRF, Guid usuarioPerfil)
        {
            DreCodigo = dreCodigo;
            UeCodigo = ueCodigo;
            TipoCalendarioId = tipoCalendarioId;
            EhSME = ehSME;
            UsuarioRF = usuarioRF;
            UsuarioPerfil = usuarioPerfil;
        }

        public string DreCodigo { get; set; }
        public string UeCodigo { get; set; }
        public long TipoCalendarioId { get; set; }
        public bool EhSME { get; set; }
        public string UsuarioRF { get; set; }
        public Guid UsuarioPerfil { get; set; }
    }
}
