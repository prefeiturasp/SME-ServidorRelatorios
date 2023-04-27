using MediatR;
using SME.SR.Data;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterObservacoesDosAlunosNoHistoricoEscolarQueryHandler : IRequestHandler<ObterObservacoesDosAlunosNoHistoricoEscolarQuery, IEnumerable<FiltroHistoricoEscolarAlunosDto>>
    {
        private readonly IHistoricoEscolarObservacaoRepository historicoEscolarObservacaoRepository;
        
        public ObterObservacoesDosAlunosNoHistoricoEscolarQueryHandler(IHistoricoEscolarObservacaoRepository historicoEscolarObservacaoRepository)
        {
            this.historicoEscolarObservacaoRepository = historicoEscolarObservacaoRepository ?? throw new ArgumentNullException(nameof(historicoEscolarObservacaoRepository));
        }
        public Task<IEnumerable<FiltroHistoricoEscolarAlunosDto>> Handle(ObterObservacoesDosAlunosNoHistoricoEscolarQuery request, CancellationToken cancellationToken)
        {
            return historicoEscolarObservacaoRepository.ObterPorCodigosAlunosAsync(request.CodigosAlunos);
        }
    }
}
