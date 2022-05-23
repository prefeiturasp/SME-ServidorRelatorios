using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterPareceresFinaisConsolidadoQuery : IRequest<IEnumerable<RelatorioParecerConclusivoRetornoDto>>
    {
        public int AnoLetivo { get; set; }
        public string DreCodigo { get; set; }
        public string UeCodigo { get; set; }
        public Modalidade? Modalidade { get; set; }
        public int? Semestre { get; set; }
        public string[] TurmasCodigo { get; set; }
        public string[] Anos { get; set; }
        public long ParecerConclusivoId { get; set; }
    }
}
