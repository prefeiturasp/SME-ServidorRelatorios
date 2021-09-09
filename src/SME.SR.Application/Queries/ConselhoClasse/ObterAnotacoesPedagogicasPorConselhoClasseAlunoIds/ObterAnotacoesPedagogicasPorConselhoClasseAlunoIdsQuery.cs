using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterAnotacoesPedagogicasPorConselhoClasseAlunoIdsQuery : IRequest<IEnumerable<AnotacoesPedagogicasAlunoIdsQueryDto>>
    {
        public ObterAnotacoesPedagogicasPorConselhoClasseAlunoIdsQuery(long[] conselhoClasseAlunoIds)
        {
            ConselhoClasseAlunoIds = conselhoClasseAlunoIds;
        }

        public long[] ConselhoClasseAlunoIds { get; set; }
    }
}
