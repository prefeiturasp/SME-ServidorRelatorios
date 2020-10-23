using MediatR;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Application
{
    public class GerarRelatorioControleGradeSinteticoCommand : IRequest<bool>
    {
        public GerarRelatorioControleGradeSinteticoCommand(RelatorioControleGradeFiltroDto filtros, Guid codigoCorrelacao)
        {
            Filtros = filtros;
            CodigoCorrelacao = codigoCorrelacao;
        }

        public RelatorioControleGradeFiltroDto Filtros { get; set; }
        public Guid CodigoCorrelacao { get; set; }
    }
}
