using System;
using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class LeituraComunicadoResponsaveoDto
    {
        public LeituraComunicadoResponsaveoDto()
        {
        }

        public string ResponsavelId { get; set; }

        public string Nome { get; set; }
        public string TipoResponsavel { get; set; }
        public string CPF { get; set; }
        public string Contato { get; set; }
    }
}
