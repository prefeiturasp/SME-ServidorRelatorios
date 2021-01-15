using System;

namespace SME.SR.Infra
{
    public class FiltroUsuarioDto
    {
        public string Dre { get; set; }
        public string Ue { get; set; }
        public string Usuario { get; set; }
        public string RF { get; set; }
        public DateTime Data => DateTime.Now;
    }
}
