using MediatR;

namespace SME.SR.Application
{
    public class ObterBimestrePeriodoFechamentoAtualQuery : IRequest<int>
    {
        public ObterBimestrePeriodoFechamentoAtualQuery(int anoLetivo)
        {
            AnoLetivo = anoLetivo;
        }

        public int AnoLetivo { get; set; }
    }
}
