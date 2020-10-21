using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class RelatorioControleGradeFiltroDto
    {
        public IEnumerable<long> Turmas { get; set; }
        public IEnumerable<long> ComponentesCurriculares { get; set; }
        public IEnumerable<int> Bimestres { get; set; }
        public ModeloRelatorio Modelo { get; set; }
    }
}
