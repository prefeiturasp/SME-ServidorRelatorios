using MediatR;
using SME.SR.Data.Interfaces;
using SME.SR.Infra.Dtos.Relatorios.CDEP;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application.Queries.CDEP.ObterRelatorioCDEPControleAcervo
{
    public class ObterRelatorioCDEPControleAcervoQueryHandler : IRequestHandler<ObterRelatorioCDEPControleAcervoQuery, IEnumerable<ControleAcervoDTO>>
    {
        private readonly IRelatorioControleLivrosRepository relatorioControleLivrosRepository;

        public ObterRelatorioCDEPControleAcervoQueryHandler(IRelatorioControleLivrosRepository relatorioControleLivrosRepository)
        {
            this.relatorioControleLivrosRepository = relatorioControleLivrosRepository;
        }

        public async Task<IEnumerable<ControleAcervoDTO>> Handle(ObterRelatorioCDEPControleAcervoQuery request, CancellationToken cancellationToken)
        {
            return await relatorioControleLivrosRepository.ObterRelatorioControleAcervos(request.filtros.TiposAcervosPermitidos, request.filtros.TipoAcervo, request.filtros.SituacaoAcervo);
        }
    }
}
