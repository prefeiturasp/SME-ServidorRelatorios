using MediatR;
using SME.SR.Workers.SGP.Commons;
using SME.SR.Workers.SGP.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Workers.SGP.Queries
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
