using MediatR;
using SME.SR.Infra;

namespace SME.SR.Application
{
    public class ObterRelatorioNotasEConceitosFinaisPdfQuery : IRequest<RelatorioNotasEConceitosFinaisDto>
    {
        public FiltroRelatorioNotasEConceitosFinaisDto FiltroRelatorioNotasEConceitosFinais { get; set; }

        public ObterRelatorioNotasEConceitosFinaisPdfQuery(FiltroRelatorioNotasEConceitosFinaisDto filtroRelatorioNotasEConceitosFinais)
        {
            this.FiltroRelatorioNotasEConceitosFinais = FiltroRelatorioNotasEConceitosFinais;
        }
    }
}
