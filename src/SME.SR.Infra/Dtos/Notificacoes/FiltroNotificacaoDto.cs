using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class FiltroNotificacaoDto
    {
        public string Dre { get; set; }
        public string Ue { get; set; }
        public string Professor { get; set; }
        public string RF { get; set; }
        public DateTime Data => DateTime.Now;
    }
}
