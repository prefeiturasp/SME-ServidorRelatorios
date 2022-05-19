using MediatR;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterSemestreUltimoAcompanhamentoGeradoQueryHandler : IRequestHandler<ObterSemestreUltimoAcompanhamentoGeradoQuery, UltimoSemestreAcompanhamentoGeradoDto>
    {
        private readonly IAcompanhamentoAprendizagemRepository acompanhamentoAprendizagemRepository;

        public ObterSemestreUltimoAcompanhamentoGeradoQueryHandler(IAcompanhamentoAprendizagemRepository acompanhamentoAprendizagemRepository)
        {
            this.acompanhamentoAprendizagemRepository = acompanhamentoAprendizagemRepository ?? throw new ArgumentNullException(nameof(acompanhamentoAprendizagemRepository));
        }

        public async Task<UltimoSemestreAcompanhamentoGeradoDto> Handle(ObterSemestreUltimoAcompanhamentoGeradoQuery request, CancellationToken cancellationToken)
                    => await acompanhamentoAprendizagemRepository.ObterUltimoSemestreAcompanhamentoGerado(request.AlunoCodigo);

    }
}
