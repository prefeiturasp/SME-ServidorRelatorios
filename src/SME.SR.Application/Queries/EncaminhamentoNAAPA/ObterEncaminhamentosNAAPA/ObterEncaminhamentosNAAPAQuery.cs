using MediatR;
using SME.SR.Infra;
using SME.SR.Infra.EncaminhamentoNaapa;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterEncaminhamentosNAAPAQuery : IRequest<IEnumerable<EncaminhamentoNaapaDto>>
    {
        public ObterEncaminhamentosNAAPAQuery(FiltroRelatorioEncaminhamentoNaapaDetalhadoDto filtroRelatorioEncaminhamentoNaapaDetalhadoDto)
        {
            this.filtroRelatorioEncaminhamentoNaapaDetalhadoDto = filtroRelatorioEncaminhamentoNaapaDetalhadoDto;
        }

        public FiltroRelatorioEncaminhamentoNaapaDetalhadoDto filtroRelatorioEncaminhamentoNaapaDetalhadoDto { get; }
    }
}
