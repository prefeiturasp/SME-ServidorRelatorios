using MediatR;
using SME.SR.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterDescricaoParecerEmAprovacaoQueryHandler : IRequestHandler<ObterDescricaoParecerEmAprovacaoQuery, string>
    {
        private readonly IParecerConclusivoRepository parecerConclusivoRepository;

        public ObterDescricaoParecerEmAprovacaoQueryHandler(IParecerConclusivoRepository usuarioRepository)
        {
            this.parecerConclusivoRepository = usuarioRepository ?? throw new ArgumentNullException(nameof(usuarioRepository));
        }

        public async Task<string> Handle(ObterDescricaoParecerEmAprovacaoQuery request, CancellationToken cancellationToken)
            => await parecerConclusivoRepository.ObterDescricaoParecerEmAprovacao(request.CodigoAluno, request.AnoLetivo);
    }
}
