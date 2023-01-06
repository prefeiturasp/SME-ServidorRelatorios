using MediatR;
using SME.SR.Infra;
using System;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class GerarRelatorioHtmlPDFEncaminhamentoAeeCommand : IRequest
    {
        public GerarRelatorioHtmlPDFEncaminhamentoAeeCommand(CabecalhoEncaminhamentoAeeDto cabecalho, List<AgrupamentoEncaminhamentoAeeDreUeDto> agrupamentos, Guid codigoCorrelacao)
        {
            Cabecalho = cabecalho;
            Agrupamentos = agrupamentos;
            CodigoCorrelacao = codigoCorrelacao;
        }

        public CabecalhoEncaminhamentoAeeDto Cabecalho { get; set; }
        public List<AgrupamentoEncaminhamentoAeeDreUeDto> Agrupamentos { get; set; }
        public Guid CodigoCorrelacao { get; set; }
    }
}
