using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra.Dtos.Relatorios.ConselhoClasse
{
    public class ConselhoClasseAtaFinalCelulaDto
    {
        public int GrupoMatriz { get; set; }

        public int ComponenteCurricular { get; set; }

        public int Coluna { get; set; }

        public string Valor { get; set; }
    }
}
