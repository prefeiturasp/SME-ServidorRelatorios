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
    public class ObterDadosAlunosHistoricoEscolarQueryHandler : IRequestHandler<ObterDadosAlunosHistoricoEscolarQuery, IEnumerable<AlunoHistoricoEscolar>>
    {
        private IAlunoRepository _alunoRepository;

        public ObterDadosAlunosHistoricoEscolarQueryHandler(IAlunoRepository alunoRepository)
        {
            this._alunoRepository = alunoRepository ?? throw new ArgumentNullException(nameof(alunoRepository));
        }

        public async Task<IEnumerable<AlunoHistoricoEscolar>> Handle(ObterDadosAlunosHistoricoEscolarQuery request, CancellationToken cancellationToken)
        {
            var dadosAlunosHistorico = await _alunoRepository.ObterDadosAlunoHistoricoEscolar(request.CodigosAluno);

            if (dadosAlunosHistorico == null || !dadosAlunosHistorico.Any())
                throw new NegocioException("Não foram encontrados os dados dos alunos informados!");

            return dadosAlunosHistorico;
        }
    }
}
