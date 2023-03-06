using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterResumoEncaminhamentosNAAPAQuery : IRequest<IEnumerable<EncaminhamentoNAAPASimplesDto>>
    {
        public ObterResumoEncaminhamentosNAAPAQuery(FiltroRelatorioEncaminhamentoNAAPADto filtro)
        {
            Filtro = filtro;
        }

        public FiltroRelatorioEncaminhamentoNAAPADto Filtro { get; }
    }
}
