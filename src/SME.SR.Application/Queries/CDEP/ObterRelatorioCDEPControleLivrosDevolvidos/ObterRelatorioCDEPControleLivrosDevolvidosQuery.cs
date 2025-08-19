using MediatR;
using SME.SR.Infra.Dtos.Relatorios.CDEP;
using System.Collections.Generic;

namespace SME.SR.Application.Queries.CDEP.ObterRelatorioCDEPControleLivrosDevolvidos
{
    public class ObterRelatorioCDEPControleLivrosDevolvidosQuery : IRequest<IEnumerable<AcervoDevolucaoDto>>
    {
        public FiltroRelatorioControleDevolucaoLivro Filtros { get; set; }
    }
}
