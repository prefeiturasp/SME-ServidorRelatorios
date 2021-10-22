using System;
using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioFrequenciaIndividualAlunosDto
    {
        public string NomeAluno { get; set; }
        public List<RelatorioFrequenciaIndividualBimestresDto> Bimestres { get; set; }
    }
}
