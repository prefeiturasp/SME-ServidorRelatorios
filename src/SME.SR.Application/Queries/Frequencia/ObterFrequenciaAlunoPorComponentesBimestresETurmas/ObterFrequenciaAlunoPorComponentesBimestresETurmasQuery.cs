using MediatR;
using SME.SR.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Application
{
    public class ObterFrequenciaAlunoPorComponentesBimestresETurmasQuery: IRequest<IEnumerable<FrequenciaAluno>>
    {
        public ObterFrequenciaAlunoPorComponentesBimestresETurmasQuery(IEnumerable<string> turmasCodigos, IEnumerable<int> bimestres, IEnumerable<long> componentesCurriculares)
        {
            TurmasCodigos = turmasCodigos;
            Bimestres = bimestres;
            ComponentesCurriculares = componentesCurriculares;
        }

        public IEnumerable<string> TurmasCodigos { get; set; }
        public IEnumerable<int> Bimestres { get; set; }
        public IEnumerable<long> ComponentesCurriculares { get; set; }
    }
}
