using MediatR;
using SME.SR.Infra.Dtos.Relatorios.CDEP;
using System.Collections.Generic;

namespace SME.SR.Application.Queries.CDEP.ObterRelatorioCDEPControleAcervoAutor
{
    public class ObterRelatorioCDEPControleAcervoAutorQuery : IRequest<IEnumerable<ControleAcervoAutorDTO>>
    {
        public FiltroRelatorioControleAcervoAutor Filtros { get; set; }
    }
}
