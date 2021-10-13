using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterAlunosPorTurmasAnosAnterioresQuery : IRequest<IEnumerable<AlunoDaTurmaDto>>
    {
        public ObterAlunosPorTurmasAnosAnterioresQuery(IEnumerable<long> turmasId)
        {
            TurmasId = turmasId;
        }

        public IEnumerable<long> TurmasId { get; set; }
    }
}
