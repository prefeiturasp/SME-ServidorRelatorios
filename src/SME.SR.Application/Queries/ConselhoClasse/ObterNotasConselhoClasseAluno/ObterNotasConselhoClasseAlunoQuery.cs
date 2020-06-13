using MediatR;
using SME.SR.Data;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterNotasConselhoClasseAlunoQuery : IRequest<IEnumerable<NotaConceitoBimestreComponente>>
    {
        public long ConselhoClasseId { get; set; }
        public string CodigoAluno { get; set; }
    }
}
