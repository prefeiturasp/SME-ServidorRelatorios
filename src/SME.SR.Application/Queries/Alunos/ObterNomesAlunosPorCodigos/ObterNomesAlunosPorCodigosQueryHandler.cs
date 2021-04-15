using MediatR;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterNomesAlunosPorCodigosQueryHandler : IRequestHandler<ObterNomesAlunosPorCodigosQuery, IEnumerable<AlunoNomeDto>>
    {
        private readonly IAlunoRepository alunoRepository;

        public ObterNomesAlunosPorCodigosQueryHandler(IAlunoRepository alunoRepository)
        {
            this.alunoRepository = alunoRepository ?? throw new ArgumentNullException(nameof(alunoRepository));
        }

        public async Task<IEnumerable<AlunoNomeDto>> Handle(ObterNomesAlunosPorCodigosQuery request, CancellationToken cancellationToken)
            => await alunoRepository.ObterNomesAlunosPorCodigos(request.Codigos);
    }
}
