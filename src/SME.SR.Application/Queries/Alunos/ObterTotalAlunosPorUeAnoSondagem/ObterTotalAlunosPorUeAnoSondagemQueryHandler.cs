using MediatR;
using SME.SR.Data.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterTotalAlunosPorUeAnoSondagemQueryHandler : IRequestHandler<ObterTotalAlunosPorUeAnoSondagemQuery, int>
    {
        private readonly IAlunoRepository alunoRepository;

        public ObterTotalAlunosPorUeAnoSondagemQueryHandler(ITurmaRepository turmaRepository, IAlunoRepository alunoRepository)
        {
            this.alunoRepository = alunoRepository ?? throw new ArgumentNullException(nameof(alunoRepository));
        }
    
        public async Task<int> Handle(ObterTotalAlunosPorUeAnoSondagemQuery request, CancellationToken cancellationToken)
        {
            return await alunoRepository
                .ObterTotalAlunosPorTurmasDataSituacaoMatriculaAsync(request.Ano, request.UeCodigo, request.AnoLetivo, request.DreCodigo, request.DataReferencia, request.Modalidades);
        }
    }
}
