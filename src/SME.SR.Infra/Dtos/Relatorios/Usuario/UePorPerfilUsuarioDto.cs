using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class UePorPerfilUsuarioDto
    {
        public string Nome { get; set; }
        public IEnumerable<PerfilUsuarioDto> Perfis { get; set; }
        public IEnumerable<UsuarioProfessorDto> Professores { get; set; }
    }
}
