using MediatR;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterNomeAlunoPorCodigoQueryHandler : IRequestHandler<ObterNomeAlunoPorCodigoQuery, AlunoNomeDto>
    {
        private readonly IAlunoRepository alunoRepository;

        public ObterNomeAlunoPorCodigoQueryHandler(IAlunoRepository alunoRepository)
        {
            this.alunoRepository = alunoRepository ?? throw new ArgumentNullException(nameof(alunoRepository));
        }

        public async Task<IEnumerable<AlunoNomeDto>> Handle(ObterNomesAlunosPorCodigosQuery request, CancellationToken cancellationToken)
            => await alunoRepository.ObterNomesAlunosPorCodigos(request.Codigos);

        public async Task<AlunoNomeDto> Handle(ObterNomeAlunoPorCodigoQuery request, CancellationToken cancellationToken)
            => await alunoRepository.ObterNomeAlunoPorCodigo(request.Codigo);
    }
}
