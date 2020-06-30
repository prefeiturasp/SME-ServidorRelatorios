using MediatR;
using SME.SR.Data;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterRelatorioConselhoClasseTurmaQuery : IRequest<IEnumerable<RelatorioConselhoClasseBase>>
    {
        public long FechamentoTurmaId { get; set; }
        public long ConselhoClasseId { get; set; }
        public string CodigoTurma { get; set; }
        public Usuario Usuario { get; set; }
    }
}