using MediatR;
using SME.SR.Infra.Dtos.Relatorios.CDEP;
using System.Collections.Generic;

namespace SME.SR.Application.Queries.CDEP.ObterRelatorioCdepHistoricoSolicitacoes
{
    public class ObterRelatorioCdepHistoricoSolicitacaoAcervoQuery : IRequest<IEnumerable<HistoricoSolicitacaoAcervoDto>>
    {
        public FiltroRelatorioHistoricoSolicitacaoAcervo Filtros { get; set; }
    }
}
