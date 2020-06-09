using MediatR;
using SME.SR.Workers.SGP.Models;
using System.Collections.Generic;

namespace SME.SR.Workers.SGP.Queries
{
    public class ObterNotasConselhoClasseAlunoQuery : IRequest<IEnumerable<NotaConceitoBimestreComponente>>
    {
        public long ConselhoClasseId { get; set; }
        public string CodigoAluno { get; set; }
    }
}
