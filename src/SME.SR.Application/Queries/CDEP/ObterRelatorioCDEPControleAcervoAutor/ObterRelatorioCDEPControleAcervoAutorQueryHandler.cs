using MediatR;
using SME.SR.Data.Interfaces;
using SME.SR.Infra.Dtos.Relatorios.CDEP;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application.Queries.CDEP.ObterRelatorioCDEPControleAcervoAutor
{
    public class ObterRelatorioCDEPControleAcervoAutorQueryHandler : IRequestHandler<ObterRelatorioCDEPControleAcervoAutorQuery, IEnumerable<ControleAcervoAutorDTO>>
    {
        private readonly IRelatorioControleLivrosRepository relatorioControleLivrosRepository;

        public ObterRelatorioCDEPControleAcervoAutorQueryHandler(IRelatorioControleLivrosRepository relatorioControleLivrosRepository)
        {
            this.relatorioControleLivrosRepository = relatorioControleLivrosRepository;
        }


        public async Task<IEnumerable<ControleAcervoAutorDTO>> Handle(ObterRelatorioCDEPControleAcervoAutorQuery request, CancellationToken cancellationToken)
        {
            return await relatorioControleLivrosRepository.ObterRelatorioControleAcervosAutor(request.Filtros.TiposAcervosPermitidos, request.Filtros.TipoAcervo, request.Filtros.Autores);
        }
    }
}
