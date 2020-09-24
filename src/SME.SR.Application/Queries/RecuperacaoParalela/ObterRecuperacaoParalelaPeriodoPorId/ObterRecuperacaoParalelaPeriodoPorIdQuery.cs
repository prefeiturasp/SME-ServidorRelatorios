using MediatR;
using SME.SR.Infra;

namespace SME.SR.Application
{
    public class ObterRecuperacaoParalelaPeriodoPorIdQuery : IRequest<RecuperacaoParalelaPeriodoDto>
    {
        public long RecuperacaoParalelaPeriodoId { get; set; }
    }
}
