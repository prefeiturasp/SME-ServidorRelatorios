using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Application.Queries.BoletimEscolar
{
    public class ObterConselhoClassePorFechamentoTurmaIdQuery : IRequest<long>
    {
        public long FechamentoTurmaId { get; set; }
    }
}
