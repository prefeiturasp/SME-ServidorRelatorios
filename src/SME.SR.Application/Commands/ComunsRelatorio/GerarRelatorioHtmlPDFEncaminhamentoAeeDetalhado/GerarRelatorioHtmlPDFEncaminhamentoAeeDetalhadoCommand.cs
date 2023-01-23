using MediatR;
using SME.SR.Infra;
using System;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class GerarRelatorioHtmlPDFEncaminhamentoAeeDetalhadoCommand : IRequest
    {
        public GerarRelatorioHtmlPDFEncaminhamentoAeeDetalhadoCommand(List<RelatorioEncaminhamentoAeeDetalhadoDto> relatorios, Guid codigoCorrelacao)
        {
            Relatorios = relatorios;
            CodigoCorrelacao = codigoCorrelacao;
        }

        public List<RelatorioEncaminhamentoAeeDetalhadoDto> Relatorios { get; set; }
        public Guid CodigoCorrelacao { get; set; }
    }
}
