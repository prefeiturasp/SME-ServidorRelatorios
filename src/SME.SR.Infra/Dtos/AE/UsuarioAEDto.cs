using System;

namespace SME.SR.Infra
{
    public class UsuarioAEDto
    {
        public string Cpf { get; set; }
        public DateTime UltimoLogin { get; set; }
        public bool PrimeiroAcesso { get; set; }
        public bool Excluido { get; set; }
    }
}
