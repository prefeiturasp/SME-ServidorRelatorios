using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterAnotacoesAlunoQueryHandler : IRequestHandler<ObterAnotacoesAlunoQuery, IEnumerable<FechamentoAlunoAnotacaoConselho>>
    {
        private IFechamentoAlunoRepository _fechamentoAlunoRepository;

        public ObterAnotacoesAlunoQueryHandler(IFechamentoAlunoRepository fechamentoAlunoRepository)
        {
            this._fechamentoAlunoRepository = fechamentoAlunoRepository;
        }

        public async Task<IEnumerable<FechamentoAlunoAnotacaoConselho>> Handle(ObterAnotacoesAlunoQuery request, CancellationToken cancellationToken)
        {
            return await _fechamentoAlunoRepository.ObterAnotacoesTurmaAlunoBimestreAsync(request.CodigoAluno, request.FechamentoTurmaId);
        }
    }
}
