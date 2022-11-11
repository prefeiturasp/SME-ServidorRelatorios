using MediatR;
using System;
using System.Collections.Generic;
using SME.SR.Infra;

namespace SME.SR.Application
{
    public class GerarRelatorioHtmlPDFPlanoAeeCommand : IRequest
    {
        public GerarRelatorioHtmlPDFPlanoAeeCommand(IEnumerable<RelatorioPlanoAeeDto> relatorioPlanoAeeDtos, Guid codigoCorrelacao)
        {
            Relatorios = relatorioPlanoAeeDtos;
            CodigoCorrelacao = codigoCorrelacao;
        }

        public IEnumerable<RelatorioPlanoAeeDto> Relatorios { get; }
        public Guid CodigoCorrelacao { get; }
    }
}
