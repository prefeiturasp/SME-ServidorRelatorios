using MediatR;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterAlunosResponsaveisPorTurmasCodigoQueryHandler : IRequestHandler<ObterAlunosResponsaveisPorTurmasCodigoQuery, IEnumerable<AlunoResponsavelAdesaoAEDto>>
    {
        private readonly IAlunoRepository alunoRepository;

        public ObterAlunosResponsaveisPorTurmasCodigoQueryHandler(IAlunoRepository alunoRepository)
        {
            this.alunoRepository = alunoRepository;
        }
        public async Task<IEnumerable<AlunoResponsavelAdesaoAEDto>> Handle(ObterAlunosResponsaveisPorTurmasCodigoQuery request, CancellationToken cancellationToken)
        {
            return await alunoRepository.ObterAlunosResponsaveisPorTurmasCodigo(request.TurmasCodigo);
        }
    }
}
