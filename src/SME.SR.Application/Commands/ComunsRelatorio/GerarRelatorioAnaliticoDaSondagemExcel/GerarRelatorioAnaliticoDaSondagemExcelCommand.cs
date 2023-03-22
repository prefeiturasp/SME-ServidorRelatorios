using MediatR;
using SME.SR.Infra;
using System;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class GerarRelatorioAnaliticoDaSondagemExcelCommand : IRequest
    {
        public IEnumerable<RelatorioSondagemAnaliticoPorDreDto> RelatorioAnalitico { get; set; }
        public TipoSondagem TipoSondagem { get; set; }
        public Guid CodigoCorrelacao { get; set; }

        public GerarRelatorioAnaliticoDaSondagemExcelCommand(IEnumerable<RelatorioSondagemAnaliticoPorDreDto> relatorioSondagemAnaliticoPorDreDtos, TipoSondagem tipoSondagem, Guid codigoCorrelacao)
        {
            RelatorioAnalitico = relatorioSondagemAnaliticoPorDreDtos;
            TipoSondagem = tipoSondagem;
            CodigoCorrelacao = codigoCorrelacao;
        }
    }
}
