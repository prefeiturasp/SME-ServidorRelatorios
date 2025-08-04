using MediatR;
using SME.SR.Data.Interfaces.CDEP;
using SME.SR.Infra.Dtos.Relatorios.CDEP;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application.Queries.CDEP.ObterRelatorioCDEPControleLivrosEmprestadoSintetico
{
    public class ObterRelatorioCDEPControleLivrosEmprestadoSinteticoQueryHandler : IRequestHandler<ObterRelatorioCDEPControleLivrosEmprestadoSinteticoQuery, IEnumerable<AcervoSolicitacaoDto>>
    {
        private readonly IMediator mediator;
        private readonly IRelatorioControleLivrosRepository relatorioControleLivrosRepository;

        public ObterRelatorioCDEPControleLivrosEmprestadoSinteticoQueryHandler(IMediator mediator, IRelatorioControleLivrosRepository relatorioControleLivrosRepository)
        {
            this.mediator = mediator;
            this.relatorioControleLivrosRepository = relatorioControleLivrosRepository;
        }

        public async Task<IEnumerable<AcervoSolicitacaoDto>> Handle(ObterRelatorioCDEPControleLivrosEmprestadoSinteticoQuery request, CancellationToken cancellationToken)
        {
            return await relatorioControleLivrosRepository.ObterRelatorioControleLivrosSintetico(request.situacaoSolicitacaoItem);
        }
    }
}
