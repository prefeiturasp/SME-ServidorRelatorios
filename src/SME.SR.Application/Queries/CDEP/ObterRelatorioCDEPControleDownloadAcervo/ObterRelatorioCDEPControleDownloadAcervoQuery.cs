using MediatR;
using SME.SR.Infra.Dtos.Relatorios.CDEP;
using System.Collections.Generic;

namespace SME.SR.Application.Queries.CDEP.ObterRelatorioCDEPControleDownloadAcervo
{
    public class ObterRelatorioCDEPControleDownloadAcervoQuery : IRequest<IEnumerable<ControleDownloadAcervoDTO>>
    {
        public FiltroRelatorioControleDownloadAcervo Filtros { get; set; }
    }
}