using MediatR;
using SME.SR.Data.Interfaces;
using SME.SR.Infra.Dtos.Relatorios.CDEP;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application.Queries.CDEP.ObterRelatorioCDEPControleLivrosEmprestadoSintetico
{
    public class ObterRelatorioCDEPControleLivrosEmprestadoQueryHandler : IRequestHandler<ObterRelatorioCDEPControleLivrosEmprestadoQuery, IEnumerable<AcervoSolicitacaoDto>>
    {
        private readonly IRelatorioControleLivrosRepository relatorioControleLivrosRepository;

        public ObterRelatorioCDEPControleLivrosEmprestadoQueryHandler(IRelatorioControleLivrosRepository relatorioControleLivrosRepository)
        {
            this.relatorioControleLivrosRepository = relatorioControleLivrosRepository;
        }

        public async Task<IEnumerable<AcervoSolicitacaoDto>> Handle(ObterRelatorioCDEPControleLivrosEmprestadoQuery request, CancellationToken cancellationToken)
        {
            return await relatorioControleLivrosRepository.ObterRelatorioControleLivrosEmpresados(request.filtros.TiposAcervosPermitidos, 
                                                                                                 request.filtros.Solicitante, 
                                                                                                 request.filtros.Tombo, 
                                                                                                 request.filtros.SituacaoEmprestimo, 
                                                                                                 request.filtros.SomenteDevolvidos);
        }
    }
}
