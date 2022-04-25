using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterRelatorioDeFrequenciaGlobalQuery : IRequest<List<FrequenciaGlobalDto>>
    {
        public FiltroFrequenciaGlobalDto Filtro { get; set; }

        public ObterRelatorioDeFrequenciaGlobalQuery(FiltroFrequenciaGlobalDto filtros)
        {
            this.Filtro = filtros;
        }
    }
}
