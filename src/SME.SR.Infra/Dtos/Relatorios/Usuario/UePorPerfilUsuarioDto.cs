using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class UePorPerfilUsuarioDto
    {
        public string Nome { get; set; }
        public List<PerfilUsuarioDto> Perfis { get; set; }
        public List<UsuarioProfessorDto> Professores { get; set; }
    }
}
