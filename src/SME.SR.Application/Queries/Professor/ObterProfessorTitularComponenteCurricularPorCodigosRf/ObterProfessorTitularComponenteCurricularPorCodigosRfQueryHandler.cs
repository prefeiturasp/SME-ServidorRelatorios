using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterProfessorTitularComponenteCurricularPorCodigosRfQueryHandler : IRequestHandler<ObterProfessorTitularComponenteCurricularPorCodigosRfQuery, IEnumerable<ProfessorTitularComponenteCurricularDto>>
    {
        private readonly IProfessorRepository professorRepository;

        public ObterProfessorTitularComponenteCurricularPorCodigosRfQueryHandler(IProfessorRepository professorRepository)
        {
            this.professorRepository = professorRepository ?? throw new ArgumentNullException(nameof(professorRepository));
        }

        public async Task<IEnumerable<ProfessorTitularComponenteCurricularDto>> Handle(ObterProfessorTitularComponenteCurricularPorCodigosRfQuery request, CancellationToken cancellationToken)
        {
            var lstProfessores = await professorRepository.BuscarProfessorTitularComponenteCurricularPorCodigosRf(request.CodigosRf);

            return lstProfessores;
        }
    }
}
