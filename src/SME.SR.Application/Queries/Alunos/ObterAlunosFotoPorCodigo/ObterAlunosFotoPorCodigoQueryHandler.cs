using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterAlunosFotoPorCodigoQueryHandler : IRequestHandler<ObterAlunosFotoPorCodigoQuery, IEnumerable<AlunoFotoArquivoDto>>
    {
        private readonly IAlunoFotoRepository alunoFotoRepository;
        private readonly IMediator mediator;

        public ObterAlunosFotoPorCodigoQueryHandler(IAlunoFotoRepository alunoFotoRepository, IMediator mediator)
        {
            this.alunoFotoRepository = alunoFotoRepository ?? throw new ArgumentNullException(nameof(alunoFotoRepository));
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<IEnumerable<AlunoFotoArquivoDto>> Handle(ObterAlunosFotoPorCodigoQuery request, CancellationToken cancellationToken)
        {
            var alunosFoto = await alunoFotoRepository.ObterFotosDoAlunoPorCodigos(request.AlunosCodigo);

            foreach (var alunoFoto in alunosFoto)
            {
                var fotoBase64 = await mediator.Send(new TransformarArquivoBase64Command(alunoFoto.ArquivoDto));
                alunoFoto.DefinirFotoBase64(fotoBase64);
            }

            return alunosFoto;
        }
    }
}
