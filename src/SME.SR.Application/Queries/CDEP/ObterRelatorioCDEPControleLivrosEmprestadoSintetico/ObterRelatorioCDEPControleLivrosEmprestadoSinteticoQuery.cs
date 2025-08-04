using MediatR;
using SME.SR.Infra.CDEP;
using SME.SR.Infra.Dtos.Relatorios.CDEP;
using System.Collections.Generic;

namespace SME.SR.Application.Queries.CDEP.ObterRelatorioCDEPControleLivrosEmprestadoSintetico
{
    public class ObterRelatorioCDEPControleLivrosEmprestadoSinteticoQuery : IRequest<IEnumerable<AcervoSolicitacaoDto>>
    {
        public SituacaoSolicitacaoItem situacaoSolicitacaoItem { get; set; }
    }
}
