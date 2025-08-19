using MediatR;
using SME.SR.Data.Interfaces;
using SME.SR.Infra.Dtos.Relatorios.CDEP;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application.Queries.CDEP.ObterRelatorioCDEPControleLivrosDevolvidos
{
    public class ObterRelatorioCDEPControleLivrosDevolvidosQueryHandler : IRequestHandler<ObterRelatorioCDEPControleLivrosDevolvidosQuery, IEnumerable<AcervoDevolucaoDto>>
    {
        private readonly IRelatorioControleLivrosRepository relatorioControleLivrosRepository;

        public ObterRelatorioCDEPControleLivrosDevolvidosQueryHandler(IRelatorioControleLivrosRepository relatorioControleLivrosRepository)
        {
            this.relatorioControleLivrosRepository = relatorioControleLivrosRepository;
        }

        public async Task<IEnumerable<AcervoDevolucaoDto>> Handle(ObterRelatorioCDEPControleLivrosDevolvidosQuery request, CancellationToken cancellationToken)
        {
            return await relatorioControleLivrosRepository.ObterRelatorioControleDevolucaoLivros(request.Filtros.TiposAcervosPermitidos, request.Filtros.Solicitante, request.Filtros.SomenteEmAtraso);
        }
    }
}
