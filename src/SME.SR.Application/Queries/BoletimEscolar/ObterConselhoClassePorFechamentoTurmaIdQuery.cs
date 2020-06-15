using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Application
{
    public class ObterConselhoClassePorFechamentoTurmaIdQuery : IRequest<long>
    {
        public long FechamentoTurmaId { get; set; }
    }
}
