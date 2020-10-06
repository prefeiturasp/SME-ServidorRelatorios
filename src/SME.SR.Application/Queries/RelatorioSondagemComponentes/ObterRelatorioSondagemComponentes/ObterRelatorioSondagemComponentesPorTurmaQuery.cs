using System.Collections.Generic;
using MediatR;
using SME.SR.Infra;

namespace SME.SR.Application
{
    public class ObterRelatorioSondagemComponentesPorTurmaQuery : IRequest<RelatorioSondagemComponentesPorTurmaRelatorioDto>
    {
        public int DreId { get; internal set; }
        public int TurmaId { get; internal set; }
        public int Ano { get; internal set; }
        public int Semestre { get; internal set; }
    }
}
