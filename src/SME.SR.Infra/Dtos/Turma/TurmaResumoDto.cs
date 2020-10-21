using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class TurmaResumoDto
    {
        public long Id { get; set; }
        public string Nome { get; set; }
        public string Ano { get; set; }
        public Modalidade Modalidade { get; set; }

        public UeDto Ue { get; set; }
    }
}
