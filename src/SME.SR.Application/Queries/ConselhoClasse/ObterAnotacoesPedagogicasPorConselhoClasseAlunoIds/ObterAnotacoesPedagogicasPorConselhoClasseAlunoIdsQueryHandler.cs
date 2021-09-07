using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterAnotacoesPedagogicasPorConselhoClasseAlunoIdsQueryHandler : IRequestHandler<ObterAnotacoesPedagogicasPorConselhoClasseAlunoIdsQuery, IEnumerable<AnotacoesPedagogicasAlunoIdsQueryDto>>
    {
        private readonly IConselhoClasseAlunoRepository conselhoClasseAlunoRepository;

        public ObterAnotacoesPedagogicasPorConselhoClasseAlunoIdsQueryHandler(IConselhoClasseAlunoRepository conselhoClasseAlunoRepository)
        {
            this.conselhoClasseAlunoRepository = conselhoClasseAlunoRepository ?? throw new ArgumentNullException(nameof(conselhoClasseAlunoRepository));
        }

        public async Task<IEnumerable<AnotacoesPedagogicasAlunoIdsQueryDto>> Handle(ObterAnotacoesPedagogicasPorConselhoClasseAlunoIdsQuery request, CancellationToken cancellationToken)
        {
            return await conselhoClasseAlunoRepository.ObterAnotacoesPedagogicasPorConselhoClasseAlunoIdsAsync(request.ConselhoClasseAlunoIds);
        }
    }
}
