using MediatR;
using SME.SR.Infra;
using System;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class GerarRelatorioHtmlPDFHistoricoEscolarCommand : IRequest
    {
        public GerarRelatorioHtmlPDFHistoricoEscolarCommand(IEnumerable<RelatorioHistoricoEscolarDto> RelatorioHistoricoEscolarDtos, Guid codigoCorrelacao)
        {
            Relatorios = RelatorioHistoricoEscolarDtos;
            CodigoCorrelacao = codigoCorrelacao;
        }

        public IEnumerable<RelatorioHistoricoEscolarDto> Relatorios { get; }
        public Guid CodigoCorrelacao { get; }
    }
}
