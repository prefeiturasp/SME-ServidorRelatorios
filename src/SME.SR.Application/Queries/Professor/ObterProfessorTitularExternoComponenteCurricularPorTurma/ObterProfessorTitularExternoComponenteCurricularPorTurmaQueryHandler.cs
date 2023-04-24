using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterProfessorTitularExternoComponenteCurricularPorTurmaQueryHandler : IRequestHandler<ObterProfessorTitularExternoComponenteCurricularPorTurmaQuery, IEnumerable<ProfessorTitularComponenteCurricularDto>>
    {
        private readonly IProfessorRepository professorRepository;

        public ObterProfessorTitularExternoComponenteCurricularPorTurmaQueryHandler(IProfessorRepository professorRepository)
        {
            this.professorRepository = professorRepository ?? throw new ArgumentNullException(nameof(professorRepository));
        }

        public Task<IEnumerable<ProfessorTitularComponenteCurricularDto>> Handle(ObterProfessorTitularExternoComponenteCurricularPorTurmaQuery request, CancellationToken cancellationToken)
        {
            return professorRepository.BuscarProfessorTitularExternoComponenteCurricularPorTurma(request.CodigosTurma);
        }
    }
}
