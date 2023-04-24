using MediatR;
using SME.SR.Infra;
using System;

namespace SME.SR.Application
{
    public class GerarRelatorioHtmlPDFEncaminhamentoNaapaCommad : IRequest
    {
        public RelatorioEncaminhamentosNAAPADto Relatorio { get; set; }
        public Guid CodigoCorrelacao { get; set; }

        public GerarRelatorioHtmlPDFEncaminhamentoNaapaCommad(RelatorioEncaminhamentosNAAPADto relatorio, Guid codigoCorrelacao)
        {
            Relatorio = relatorio;
            CodigoCorrelacao = codigoCorrelacao;
        }
    }
}
