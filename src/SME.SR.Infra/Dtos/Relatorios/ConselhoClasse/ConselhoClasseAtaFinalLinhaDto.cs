using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra.Dtos.Relatorios.ConselhoClasse
{
    public class ConselhoClasseAtaFinalLinhaDto
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public List<ConselhoClasseAtaFinalCelulaDto> Celulas { get; set; }
    }
}
