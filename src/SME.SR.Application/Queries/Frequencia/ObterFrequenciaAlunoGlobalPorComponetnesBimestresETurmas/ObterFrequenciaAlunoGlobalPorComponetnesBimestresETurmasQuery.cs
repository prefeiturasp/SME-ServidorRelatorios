using MediatR;
using SME.SR.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Application
{
    public class ObterFrequenciaAlunoGlobalPorComponetnesBimestresETurmasQuery: IRequest<IEnumerable<FrequenciaAluno>>
    {
        public ObterFrequenciaAlunoGlobalPorComponetnesBimestresETurmasQuery(IEnumerable<Turma> turmas, IEnumerable<long> componentesCurriculares)
        {
            Turmas = turmas;
            ComponentesCurriculares = componentesCurriculares;
        }

        public IEnumerable<Turma> Turmas { get; set; }
        public IEnumerable<long> ComponentesCurriculares { get; set; }
    }
}
