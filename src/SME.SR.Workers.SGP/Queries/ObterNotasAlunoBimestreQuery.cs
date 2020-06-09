using MediatR;
using SME.SR.Workers.SGP.Models;
using System.Collections.Generic;

namespace SME.SR.Workers.SGP.Queries
{
    public class ObterNotasAlunoBimestreQuery : IRequest<IEnumerable<NotaConceitoBimestreComponente>>
    {
        public long FechamentoTurmaId { get; set; }
        public string CodigoAluno { get; set; }
        public string CodigoTurma { get; set; }
        public int? Bimestre { get; set; }
    }
}
