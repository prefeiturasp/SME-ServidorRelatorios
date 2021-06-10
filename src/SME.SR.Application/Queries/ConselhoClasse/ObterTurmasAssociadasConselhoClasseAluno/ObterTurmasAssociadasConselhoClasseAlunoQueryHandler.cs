using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterTurmasAssociadasConselhoClasseAlunoQueryHandler : IRequestHandler<ObterTurmasAssociadasConselhoClasseAlunoQuery, IEnumerable<TurmaComplementarConselhoClasseAluno>>
    {
        private readonly IConselhoClasseAlunoTurmaComplementarRepository ccAlunoTurmaComplementarRepository;

        public ObterTurmasAssociadasConselhoClasseAlunoQueryHandler(IConselhoClasseAlunoTurmaComplementarRepository ccAlunoTurmaComplementarRepository)
        {
            this.ccAlunoTurmaComplementarRepository = ccAlunoTurmaComplementarRepository ?? throw new System.ArgumentNullException(nameof(ccAlunoTurmaComplementarRepository));
        }

        public async Task<IEnumerable<TurmaComplementarConselhoClasseAluno>> Handle(ObterTurmasAssociadasConselhoClasseAlunoQuery request, CancellationToken cancellationToken)
        {
            return await ccAlunoTurmaComplementarRepository.ObterTurmasPorConselhoClasseAlunoIds(request.ConselhoClasseAlunoIds);
        }
    }
}
