using MediatR;
using System;
using SME.SR.Infra;

namespace SME.SR.Application
{
    public class GerarRelatorioHtmlPDFPlanoAeeCommand : IRequest
    {
        public GerarRelatorioHtmlPDFPlanoAeeCommand(RelatorioPlanoAeeDto relatorio, Guid codigoCorrelacao)
        {
            Relatorio = relatorio;
            CodigoCorrelacao = codigoCorrelacao;
        }

        public RelatorioPlanoAeeDto Relatorio { get; }
        public Guid CodigoCorrelacao { get; }
    }
}
