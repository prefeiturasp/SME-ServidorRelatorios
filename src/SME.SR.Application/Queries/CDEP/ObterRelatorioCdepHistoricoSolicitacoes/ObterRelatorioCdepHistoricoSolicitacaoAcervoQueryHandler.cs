using MediatR;
using SME.SR.Infra.Dtos.Relatorios.CDEP;
using System.Collections.Generic;

namespace SME.SR.Application.Queries.CDEP.ObterRelatorioCdepHistoricoSolicitacoes
{
    public class ObterRelatorioCdepHistoricoSolicitacaoAcervoQueryHandler : IRequestHandler<ObterRelatorioCdepHistoricoSolicitacaoAcervoQuery, IEnumerable<HistoricoSolicitacaoAcervoDto>>
    {
        private readonly Data.Interfaces.IRelatorioControleLivrosRepository relatorioControleLivrosRepository;
        public ObterRelatorioCdepHistoricoSolicitacaoAcervoQueryHandler(Data.Interfaces.IRelatorioControleLivrosRepository relatorioControleLivrosRepository)
        {
            this.relatorioControleLivrosRepository = relatorioControleLivrosRepository;
        }
        public async System.Threading.Tasks.Task<IEnumerable<HistoricoSolicitacaoAcervoDto>> Handle(ObterRelatorioCdepHistoricoSolicitacaoAcervoQuery request, System.Threading.CancellationToken cancellationToken)
        {
            return await relatorioControleLivrosRepository.ObterRelatorioHistoricoSolicitacaoAcervo(request.Filtros.Solicitante,
                                                                                                   request.Filtros.SituacaoSolicitacao,
                                                                                                   request.Filtros.TipoAcervo,
                                                                                                   request.Filtros.DataInicio,
                                                                                                   request.Filtros.DataFim);
        }
    }
}
