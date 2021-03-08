using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class FiltroRelatorioDevolutivasDto
    {
        public long UeId { get; set; }
        public IEnumerable<long> Turmas { get; set; }
        public IEnumerable<int> Bimestres { get; set; }
        public string UsuarioNome { get; set; }
        public string UsuarioRF { get; set; }
    }
}
