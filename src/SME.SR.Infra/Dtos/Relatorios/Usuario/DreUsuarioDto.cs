using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class DreUsuarioDto
    {
        public string Nome { get; set; }
        public IEnumerable<PerfilUsuarioDto> Perfis { get; set; }
        public IEnumerable<UePorPerfilUsuarioDto> Ues { get; set; }
        public IEnumerable<HistoricoReinicioSenhaDto> HistoricoReinicioSenha { get; set; }

    }
}
