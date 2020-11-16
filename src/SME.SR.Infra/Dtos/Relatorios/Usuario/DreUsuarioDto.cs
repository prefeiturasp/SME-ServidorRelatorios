using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class DreUsuarioDto
    {
        public string Nome { get; set; }
        public List<PerfilUsuarioDto> Perfis { get; set; }
        public List<UePorPerfilUsuarioDto> Ues { get; set; }
        public List<HistoricoReinicioSenhaDto> HistoricoReinicioSenha { get; set; }

    }
}
