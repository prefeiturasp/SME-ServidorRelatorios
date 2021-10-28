using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterAlunosPorTurmasQuery : IRequest<IEnumerable<AlunoDaTurmaDto>>
    {
        public ObterAlunosPorTurmasQuery(IEnumerable<long> turmasId)
        {
            TurmasId = turmasId;
        }
        public IEnumerable<long> TurmasId { get; set; }
    }
}
