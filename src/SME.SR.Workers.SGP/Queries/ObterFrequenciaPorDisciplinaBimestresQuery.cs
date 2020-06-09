using MediatR;
using SME.SR.Workers.SGP.Models;
using System.Collections.Generic;

namespace SME.SR.Workers.SGP.Queries
{
    public class ObterFrequenciaPorDisciplinaBimestresQuery : IRequest<IEnumerable<FrequenciaAluno>>
    {
        public string CodigoTurma { get; set; }
        public string CodigoAluno { get; set; }
        public int? Bimestre { get; set; }
    }
}
