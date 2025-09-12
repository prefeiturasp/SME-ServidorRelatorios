using MediatR;
using SME.SR.Infra.Dtos.Relatorios.CDEP;
using System.Collections.Generic;

namespace SME.SR.Application.Queries.CDEP.ObterRelatorioCDEPControleEditora
{
    public class ObterRelatorioCDEPControleEditoraQuery : IRequest<IEnumerable<ControleEditoraDTO>>
    {
        public FiltroRelatorioControleEditora filtros { get; set; }
    }
}
