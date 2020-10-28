using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class AulaExcedidaDto
    {
        public AulaExcedidaDto()
        {
        }

        public string Data { get; set; }
        public int Quantidade { get; set; }
        public string Professor { get; set; }
    }
}
