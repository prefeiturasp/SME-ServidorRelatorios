using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Data
{
    public class PrioridadePerfil
    {
        public Guid CodigoPerfil { get; set; }
        public string NomePerfil { get; set; }
        public int Ordem { get; set; }
        public TipoPerfil Tipo { get; set; }
    }
}
