using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class FiltroNotificacaoDto
    {
        public IEnumerable<long> DREs { get; set; }
        public IEnumerable<long> UEs { get; set; }
        public string Professor { get; set; }
        public string RF { get; set; }
        public DateTime Data => DateTime.Now;
    }
}
