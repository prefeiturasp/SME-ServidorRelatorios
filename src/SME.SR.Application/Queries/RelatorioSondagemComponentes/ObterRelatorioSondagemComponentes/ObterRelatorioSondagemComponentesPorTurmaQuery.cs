using System.Collections.Generic;
using MediatR;
using SME.SR.Infra;

namespace SME.SR.Application
{
    public class ObterRelatorioSondagemComponentesPorTurmaQuery : IRequest<IEnumerable<RelatorioSondagemComponentesPorTurmaRetornoQueryDto>>
    {
        public int DreId { get; internal set; }
        public int TurmaId { get; internal set; }
        public int UeId { get; internal set; }
        public int Ano { get; internal set; }
    }
}
