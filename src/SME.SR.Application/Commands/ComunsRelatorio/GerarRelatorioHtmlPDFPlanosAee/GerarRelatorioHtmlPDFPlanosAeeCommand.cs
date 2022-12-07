using MediatR;
using SME.SR.Infra;
using System;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class GerarRelatorioHtmlPDFPlanosAeeCommand : IRequest
    {
        public CabecalhoPlanosAeeDto Cabecalho { get; set; } 
        public List<AgrupamentoDreUeDto> Agrupamentos { get; set; }
        public Guid CodigoCorrelacao { get; set; }

        public GerarRelatorioHtmlPDFPlanosAeeCommand(CabecalhoPlanosAeeDto cabecalho, List<AgrupamentoDreUeDto> agrupamentos, Guid codigoCorrelacao)
        {
            Cabecalho = cabecalho;
            Agrupamentos = agrupamentos;
            CodigoCorrelacao = codigoCorrelacao;
        }
    }
}
