using MediatR;
using SME.SR.Infra;
using SME.SR.Infra.EncaminhamentoNaapa;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterEncaminhamentosNAAPAQuery : IRequest<IEnumerable<EncaminhamentoNAAPADto>>
    {
        public ObterEncaminhamentosNAAPAQuery(FiltroRelatorioEncaminhamentoNAAPADetalhadoDto filtroRelatorioEncaminhamentoNaapaDetalhadoDto)
        {
            this.filtroRelatorioEncaminhamentoNaapaDetalhadoDto = filtroRelatorioEncaminhamentoNaapaDetalhadoDto;
        }

        public FiltroRelatorioEncaminhamentoNAAPADetalhadoDto filtroRelatorioEncaminhamentoNaapaDetalhadoDto { get; }
    }
}
