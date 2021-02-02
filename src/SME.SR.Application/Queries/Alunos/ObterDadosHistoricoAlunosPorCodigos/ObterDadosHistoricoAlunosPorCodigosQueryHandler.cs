using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterDadosHistoricoAlunosPorCodigosQueryHandler : IRequestHandler<ObterDadosHistoricoAlunosPorCodigosQuery, IEnumerable<AlunoHistoricoEscolar>>
    {
        private readonly IAlunoRepository alunoRepository;

        public ObterDadosHistoricoAlunosPorCodigosQueryHandler(IAlunoRepository alunoRepository)
        {
            this.alunoRepository = alunoRepository ?? throw new ArgumentNullException(nameof(alunoRepository));
        }

        public async Task<IEnumerable<AlunoHistoricoEscolar>> Handle(ObterDadosHistoricoAlunosPorCodigosQuery request, CancellationToken cancellationToken)
        {
            var dadosAlunos = await alunoRepository.ObterDadosHistoricoAlunosPorCodigos(request.CodigosAluno);

            if (dadosAlunos == null || !dadosAlunos.Any())
                throw new NegocioException("Não foram encontrados alunos com os dados informados!");

            return dadosAlunos;
        }
    }
}
