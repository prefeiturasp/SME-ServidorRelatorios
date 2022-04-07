using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra.RelatorioPaginado.Interfaces
{
    public class SubColuna
    {
        public string Titulo { get; set; }

        public bool Chave { get; set; }

        public int ColSpan { get; set; }
    }
}
