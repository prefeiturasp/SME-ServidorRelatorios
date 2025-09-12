using MediatR;
using SME.SR.Infra.Dtos.Relatorios.CDEP;
using System.Collections.Generic;

namespace SME.SR.Application.Queries.CDEP.ObterRelatorioCDEPControleLivrosEmprestadoSintetico
{
    public class ObterRelatorioCDEPControleLivrosEmprestadoQuery : IRequest<IEnumerable<AcervoSolicitacaoDto>>
    {
        public FiltroRelatorioControleLivro filtros { get; set; }
    }
}
