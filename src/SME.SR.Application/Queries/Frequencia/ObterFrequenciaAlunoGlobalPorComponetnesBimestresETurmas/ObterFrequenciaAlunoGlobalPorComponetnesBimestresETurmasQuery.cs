using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Application
{
    public class ObterFrequenciaAlunoGlobalPorComponetnesBimestresETurmasQuery: IRequest<IEnumerable<FrequenciaAluno>>
    {
        public ObterFrequenciaAlunoGlobalPorComponetnesBimestresETurmasQuery(IEnumerable<string> turmasCodigos, IEnumerable<long> componentesCurriculares, Modalidade modalidade, long tipoCalendarioId)
        {
            TurmasCodigos = turmasCodigos;
            ComponentesCurriculares = componentesCurriculares;
            Modalidade = modalidade;
            TipoCalendarioId = tipoCalendarioId;
        }

        public IEnumerable<string> TurmasCodigos { get; set; }
        public IEnumerable<long> ComponentesCurriculares { get; set; }
        public Modalidade Modalidade { get; set; }
        public long TipoCalendarioId { get; set; }
    }
}
