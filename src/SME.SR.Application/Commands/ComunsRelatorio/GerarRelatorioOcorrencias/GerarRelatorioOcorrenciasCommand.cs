using MediatR;
using SME.SR.Infra;
using System;

namespace SME.SR.Application
{
    public class GerarRelatorioOcorrenciasCommand : IRequest
    {

        public GerarRelatorioOcorrenciasCommand(RelatorioRegistroOcorrenciasDto relatorio, Guid codigoCorrelacao)
        {
            Relatorio = relatorio;
            CodigoCorrelacao = codigoCorrelacao;
        }

        public RelatorioRegistroOcorrenciasDto Relatorio { get; }
        public Guid CodigoCorrelacao { get; }
    }
}
