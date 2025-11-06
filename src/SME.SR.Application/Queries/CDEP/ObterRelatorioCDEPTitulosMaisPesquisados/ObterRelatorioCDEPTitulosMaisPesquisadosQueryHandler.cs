using MediatR;
using SME.SR.Data.Interfaces;
using SME.SR.Infra.Dtos.Relatorios.CDEP;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application.Queries.CDEP.ObterRelatorioCDEPTitulosMaisPesquisados
{
    public class ObterRelatorioCDEPTitulosMaisPesquisadosQueryHandler : IRequestHandler<ObterRelatorioCDEPTitulosMaisPesquisadosQuery, IEnumerable<RelatorioTitulosMaisPesquisadosDto>>
    {
        private readonly IRelatorioControleLivrosRepository relatorioControleLivrosRepository;

        public ObterRelatorioCDEPTitulosMaisPesquisadosQueryHandler(IRelatorioControleLivrosRepository relatorioControleLivrosRepository)
        {
            this.relatorioControleLivrosRepository = relatorioControleLivrosRepository;
        }
        public async Task<IEnumerable<RelatorioTitulosMaisPesquisadosDto>> Handle(ObterRelatorioCDEPTitulosMaisPesquisadosQuery request, CancellationToken cancellationToken)
        {
            return await relatorioControleLivrosRepository.ObterRelatorioTitulosMaisPesquisados(request.Filtros.DataInicio, 
                                                                                               request.Filtros.DataFim, 
                                                                                               request.Filtros.TipoAcervos);
        }
    }
}
