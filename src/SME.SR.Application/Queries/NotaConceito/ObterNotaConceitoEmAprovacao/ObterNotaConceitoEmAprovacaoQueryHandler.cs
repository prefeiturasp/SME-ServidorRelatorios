using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterNotaConceitoEmAprovacaoQueryHandler : IRequestHandler<ObterNotaConceitoEmAprovacaoQuery, NotaConceitoEmAprovacaoDto>
    {
        private INotaConceitoRepository notasConceitoRepository;

        public ObterNotaConceitoEmAprovacaoQueryHandler(INotaConceitoRepository notasConceitoRepository)
        {
            this.notasConceitoRepository = notasConceitoRepository ?? throw new ArgumentException(nameof(notasConceitoRepository));
        }

        public async Task<NotaConceitoEmAprovacaoDto> Handle(ObterNotaConceitoEmAprovacaoQuery request, CancellationToken cancellationToken)
        {
            return await notasConceitoRepository.ObterNotaConceitoEmAprovacao(request.CodigoAluno, request.ConselhoClasseAlunoId);
        }
    
    }
}
