using System;
using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class LeituraComunicadoResponsavelDto
    {
        public LeituraComunicadoResponsavelDto()
        {
        }
        public long UsuarioId { get; set; }
        public string UsuarioCPF { get; set; }
        public long ComunicadoId { get; set; }
        public string ResponsavelId { get; set; }
        public string AlunoId { get; set; }
        public string ResponsavelNome { get; set; }
        public string TipoResponsavel { get; set; }
        public string CPF { get; set; }
        public string Contato { get; set; }
    }
}
