using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Data
{
    public class Itinerancia
    {
        public long Id { get; set; }
        public DateTime DataVisita { get; set; }
        public DateTime DataRetorno { get; set; }
        public int Situacao { get; set; }
        public int AnoLetivo { get; set; }
        public Ue Ue { get; set; }
    }
}
