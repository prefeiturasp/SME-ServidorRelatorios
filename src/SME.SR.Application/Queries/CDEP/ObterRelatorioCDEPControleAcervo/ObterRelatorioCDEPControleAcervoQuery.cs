using MediatR;
using SME.SR.Infra.Dtos.Relatorios.CDEP;
using System.Collections.Generic;

namespace SME.SR.Application.Queries.CDEP.ObterRelatorioCDEPControleAcervo
{
    public class ObterRelatorioCDEPControleAcervoQuery : IRequest<IEnumerable<ControleAcervoDTO>>
    {
        public FiltroRelatorioControleAcervo filtros { get; set; }
    }
}
