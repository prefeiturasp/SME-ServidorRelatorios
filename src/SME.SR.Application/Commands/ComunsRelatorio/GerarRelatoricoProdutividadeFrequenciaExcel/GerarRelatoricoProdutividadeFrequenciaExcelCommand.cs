using MediatR;
using SME.SR.Infra;
using System;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class GerarRelatoricoProdutividadeFrequenciaExcelCommand : IRequest
    {
        public IEnumerable<ConsolidacaoProdutividadeFrequenciaDto> Consolidacoes { get; set; }
        public FiltroRelatorioProdutividadeFrequenciaDto Filtro { get; set; }
        public Guid CodigoCorrelacao { get; set; }

        public GerarRelatoricoProdutividadeFrequenciaExcelCommand(IEnumerable<ConsolidacaoProdutividadeFrequenciaDto> consolidacoes, FiltroRelatorioProdutividadeFrequenciaDto filtro, Guid codigoCorrelacao)
        {
            Consolidacoes = consolidacoes;
            Filtro = filtro;
            CodigoCorrelacao = codigoCorrelacao;
        }
    }
}
