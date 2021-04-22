using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterTurmasAssociadasConselhoClasseAlunoQuery : IRequest<IEnumerable<TurmaComplementarConselhoClasseAluno>>
    {
        public long[] ConselhoClasseAlunoIds { get; set; }

        public ObterTurmasAssociadasConselhoClasseAlunoQuery(long[] conselhoClasseAlunoIds)
        {
            ConselhoClasseAlunoIds = conselhoClasseAlunoIds;
        }
    }
}
