using MediatR;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Application
{
    public class GerarRelatorioNotificacaoCommand : IRequest<bool>
    {
        public GerarRelatorioNotificacaoCommand(FiltroNotificacaoDto filtros, Guid codigoCorrelacao)
        {
            Filtros = filtros;
            CodigoCorrelacao = codigoCorrelacao;
        }

        public FiltroNotificacaoDto Filtros { get; set; }
        public Guid CodigoCorrelacao { get; set; }
    }
}
