using MediatR;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterTotalAlunosPorUeAnoSondagemQueryHandler : IRequestHandler<ObterTotalAlunosPorUeAnoSondagemQuery, int>
    {
        private readonly ITurmaRepository turmaRepository;
        private readonly IAlunoRepository alunoRepository;

        public ObterTotalAlunosPorUeAnoSondagemQueryHandler(ITurmaRepository turmaRepository, IAlunoRepository alunoRepository)
        {
            this.turmaRepository = turmaRepository ?? throw new ArgumentNullException(nameof(turmaRepository));
            this.alunoRepository = alunoRepository ?? throw new ArgumentNullException(nameof(alunoRepository));
        }
    
        public async Task<int> Handle(ObterTotalAlunosPorUeAnoSondagemQuery request, CancellationToken cancellationToken)
        {
            return await alunoRepository.ObterTotalAlunosPorTurmasDataSituacaoMatriculaAsync(request.Ano, request.UeCodigo, request.AnoLetivo, request.DreCodigo, request.DataReferencia);
        }
    }
}
