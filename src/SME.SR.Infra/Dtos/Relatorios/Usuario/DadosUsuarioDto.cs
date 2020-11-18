using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SME.SR.Infra
{
    public class DadosUsuarioDto
    {
        public string DreCodigo { get; set; }
        public string Dre { get; set; }
        public string Ue { get; set; }
        public Guid PerfilGuid { get; set; }
        public string Perfil { get; set; }
        public TipoPerfil TipoPerfil { get; set; }
        public string Rf { get; set; }
        public string Nome { get; set; }
        public SituacaoUsuario Situacao { get; set; }
        public DateTime UltimoAcesso { get; set; }
    }
}
