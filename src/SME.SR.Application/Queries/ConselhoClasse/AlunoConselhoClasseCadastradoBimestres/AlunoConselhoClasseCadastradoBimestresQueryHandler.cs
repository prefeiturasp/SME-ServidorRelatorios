using MediatR;
using SME.SR.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class AlunoConselhoClasseCadastradoBimestresQueryHandler : IRequestHandler<AlunoConselhoClasseCadastradoBimestresQuery, IEnumerable<int>>
    {
        private readonly IConselhoClasseRepository repository;

        public AlunoConselhoClasseCadastradoBimestresQueryHandler(IConselhoClasseRepository repository)
        {
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<IEnumerable<int>> Handle(AlunoConselhoClasseCadastradoBimestresQuery request, CancellationToken cancellationToken)
        {
            return await repository.ObterBimestresPorAlunoCodigo(request.CodigoAluno, request.AnoLetivo, request.Modalidade, request.Semestre, request.CodigoTurma);
        }
    }
}
