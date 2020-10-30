using MediatR;
using SME.SR.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Application
{
    public class ObterTurmasPorIdsQuery : IRequest<IEnumerable<Turma>>
    {
        public ObterTurmasPorIdsQuery(long[] ids)
        {
            Ids = ids;
        }

        public long[] Ids { get; set; }
    }
}
