using MediatR;
using SME.SR.Infra.Dtos.Relatorios.CDEP;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application.Queries.CDEP.ObterRelatorioCDEPControleDownloadAcervo
{
    public class ObterRelatorioCDEPControleDownloadAcervoQueryHandler : IRequestHandler<ObterRelatorioCDEPControleDownloadAcervoQuery, IEnumerable<ControleDownloadAcervoDTO>>
    {
        private readonly Data.Interfaces.IRelatorioControleLivrosRepository relatorioControleLivrosRepository;
        public ObterRelatorioCDEPControleDownloadAcervoQueryHandler(Data.Interfaces.IRelatorioControleLivrosRepository relatorioControleLivrosRepository)
        {
            this.relatorioControleLivrosRepository = relatorioControleLivrosRepository;
        }
        public async Task<IEnumerable<ControleDownloadAcervoDTO>> Handle(ObterRelatorioCDEPControleDownloadAcervoQuery request, CancellationToken cancellationToken)
        {
            return await relatorioControleLivrosRepository.ObterRelatorioControleDownloadAcervo(request.Filtros.Titulo, request.Filtros.TipoAcervo);
        }
    }
}