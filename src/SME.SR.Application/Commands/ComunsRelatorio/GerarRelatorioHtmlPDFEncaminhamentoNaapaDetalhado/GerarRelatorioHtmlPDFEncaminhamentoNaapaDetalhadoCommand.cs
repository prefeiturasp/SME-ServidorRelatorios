using MediatR;
using SME.SR.Infra;
using System;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class GerarRelatorioHtmlPDFEncaminhamentoNaapaDetalhadoCommand : IRequest
    {
        public List<RelatorioEncaminhamentoNAAPADetalhadoDto> RelatorioEncaminhamentoNAAPADetalhadoDtos { get; set; }
        public Guid CodigoCorrelacao { get; set; }
        public bool ImprimirAnexos{ get; set; }

        public GerarRelatorioHtmlPDFEncaminhamentoNaapaDetalhadoCommand(List<RelatorioEncaminhamentoNAAPADetalhadoDto> relatorioDtos, bool imprimirAnexos, Guid codigoCorrelacao)
        {
            RelatorioEncaminhamentoNAAPADetalhadoDtos = relatorioDtos;
            CodigoCorrelacao = codigoCorrelacao;
            ImprimirAnexos = imprimirAnexos;
        }
    }
}
