using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System;

namespace SME.SR.Application
{
    public class ObterRelatorioImpressaoCalendarioQuery : IRequest<RelatorioImpressaoCalendarioDto>
    {
        public ObterRelatorioImpressaoCalendarioQuery(Dre dre, Ue ue,  TipoCalendarioDto tipoCalendario, bool ehSME, string usuarioRF, Guid usuarioPerfil, bool consideraPendenteAprovacao, bool podeVisualizarEventosOcorrenciaDre)
        {
            Dre = dre;
            Ue = ue;            
            TipoCalendario = tipoCalendario;
            EhSME = ehSME;
            UsuarioRF = usuarioRF;
            UsuarioPerfil = usuarioPerfil;
            ConsideraPendenteAprovacao = consideraPendenteAprovacao;
            PodeVisualizarEventosOcorrenciaDre = podeVisualizarEventosOcorrenciaDre;
        }

        public Dre Dre { get; set; }
        public Ue Ue { get; set; }
        public TipoCalendarioDto TipoCalendario  { get; set; }
        public bool EhSME { get; set; }
        public string UsuarioRF { get; set; }
        public Guid UsuarioPerfil { get; set; }        
        
        public bool ConsideraPendenteAprovacao { get; internal set; }
        public bool PodeVisualizarEventosOcorrenciaDre { get; internal set; }
    }
}
