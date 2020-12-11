using MediatR;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Application
{
    public class ObterDadosLeituraComunicadosQuery : IRequest<IEnumerable<LeituraComunicadoDto>>
    {
        public ObterDadosLeituraComunicadosQuery(FiltroRelatorioLeituraComunicadosDto filtro)
        {
            Filtro = filtro;
        }

        public FiltroRelatorioLeituraComunicadosDto Filtro { get; set; }
    }
}
