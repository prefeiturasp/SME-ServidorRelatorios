using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterEncaminhamentosNAAPAQuery : IRequest<IEnumerable<EncaminhamentoNAAPADto>>
    {
        public ObterEncaminhamentosNAAPAQuery(FiltroRelatorioEncaminhamentoNAAPADto filtro)
        {
            Filtro = filtro;
        }

        public FiltroRelatorioEncaminhamentoNAAPADto Filtro { get; }
    }
}
