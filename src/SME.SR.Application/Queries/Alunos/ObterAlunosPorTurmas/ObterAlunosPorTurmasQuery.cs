using MediatR;
using SME.SR.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Application
{
    public class ObterAlunosPorTurmasQuery : IRequest<IEnumerable<Aluno>>
    {
        public IEnumerable<long> TurmasId { get; set; }
    }
}
