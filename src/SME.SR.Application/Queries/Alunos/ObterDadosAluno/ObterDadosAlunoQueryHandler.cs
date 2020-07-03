using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterDadosAlunoQueryHandler : IRequestHandler<ObterDadosAlunoQuery, Aluno>
    {
        private readonly IAlunoRepository alunoRepository;

        public ObterDadosAlunoQueryHandler(IAlunoRepository alunoRepository)
        {
            this.alunoRepository = alunoRepository ?? throw new ArgumentNullException(nameof(alunoRepository));
        }

        public async Task<Aluno> Handle(ObterDadosAlunoQuery request, CancellationToken cancellationToken)
        {           
            return await alunoRepository.ObterDados(request.CodigoTurma, request.CodigoAluno);
        }
    }
}
