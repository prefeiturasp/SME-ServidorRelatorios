using System;

namespace SME.SR.Infra
{
    public class RelatorioImpressaoCalendarioFiltroDto
    {
        public string DreCodigo { get; set; }
        public string UeCodigo { get; set; }
        public long TipoCalendarioId { get; set; }
        public bool EhSME { get; set; }
        public string UsuarioRF { get; set; }
        public Guid UsuarioPerfil { get; set; }
        public bool ConsideraPendenteAprovacao { get; set; }
        public bool PodeVisualizarEventosOcorrenciaDre { get; set; }
    }
}
