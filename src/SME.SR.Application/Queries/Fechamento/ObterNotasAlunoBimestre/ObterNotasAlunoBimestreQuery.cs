using MediatR;
using SME.SR.Data;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterNotasAlunoBimestreQuery : IRequest<IEnumerable<NotaConceitoBimestreComponente>>
    {
        public long FechamentoTurmaId { get; set; }
        public string CodigoAluno { get; set; }
        public string CodigoTurma { get; set; }
        public int? Bimestre { get; set; }
    }
}
