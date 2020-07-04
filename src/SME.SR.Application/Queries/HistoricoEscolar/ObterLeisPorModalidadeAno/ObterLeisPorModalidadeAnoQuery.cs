using MediatR;
using SME.SR.Infra;

namespace SME.SR.Application
{
    public class ObterLeisPorModalidadeAnoQuery : IRequest<string>
    {
        public int AnoLetivo { get; set; }

        public Modalidade? Modalidade { get; set; }

    }
}
