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
    public class ObterDadosAlunosPorCodigosQueryHandler : IRequestHandler<ObterDadosAlunosPorCodigosQuery, IEnumerable<AlunoHistoricoEscolar>>
    {
        private readonly IAlunoRepository alunoRepository;

        public ObterDadosAlunosPorCodigosQueryHandler(IAlunoRepository alunoRepository)
        {
            this.alunoRepository = alunoRepository ?? throw new ArgumentNullException(nameof(alunoRepository));
        }

        public async Task<IEnumerable<AlunoHistoricoEscolar>> Handle(ObterDadosAlunosPorCodigosQuery request, CancellationToken cancellationToken)
        {
            var dadosAlunos = await alunoRepository.ObterDadosAlunosPorCodigos(request.CodigosAluno, request.AnoLetivo);

            if (dadosAlunos == null || !dadosAlunos.Any())
                throw new NegocioException("Não foram encontrados alunos com os dados informados!");

            return dadosAlunos;
        }
    }
}
