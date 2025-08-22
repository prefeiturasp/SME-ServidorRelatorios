using MediatR;
using SME.SR.Data.Interfaces;
using SME.SR.Infra.Dtos.Relatorios.CDEP;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application.Queries.CDEP.ObterRelatorioCDEPControleEditora
{
    public class ObterRelatorioCDEPControleEditoraQueryHandler : IRequestHandler<ObterRelatorioCDEPControleEditoraQuery, IEnumerable<ControleEditoraDTO>>
    {
        private readonly IRelatorioControleLivrosRepository relatorioControleLivrosRepository;

        public ObterRelatorioCDEPControleEditoraQueryHandler(IRelatorioControleLivrosRepository relatorioControleLivrosRepository)
        {
            this.relatorioControleLivrosRepository = relatorioControleLivrosRepository;
        }

        public async Task<IEnumerable<ControleEditoraDTO>> Handle(ObterRelatorioCDEPControleEditoraQuery request, CancellationToken cancellationToken)
        {
            return await relatorioControleLivrosRepository.ObterRelatorioControleEditoras(request.filtros.EditoraId);
        }
    }
}
