using MediatR;
using SME.SR.Infra.Dtos.Relatorios.CDEP;
using System.Collections.Generic;

namespace SME.SR.Application.Queries.CDEP.ObterRelatorioCDEPTitulosMaisPesquisados
{
    public class ObterRelatorioCDEPTitulosMaisPesquisadosQuery : IRequest<IEnumerable<RelatorioTitulosMaisPesquisadosDto>>
    {
        public FiltroRelatorioTitulosMaisPesquisados Filtros { get; set; }
    }
}
