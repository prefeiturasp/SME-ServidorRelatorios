using System;
using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioFrequenciaIndividualAlunosDto
    {
        public RelatorioFrequenciaIndividualAlunosDto()
        {
            Bimestres = new List<RelatorioFrequenciaIndividualBimestresDto>();
        }
        public string NomeAluno { get; set; }
        public string CodigoAluno { get; set; }
        public List<RelatorioFrequenciaIndividualBimestresDto> Bimestres { get; set; }
    }
}
