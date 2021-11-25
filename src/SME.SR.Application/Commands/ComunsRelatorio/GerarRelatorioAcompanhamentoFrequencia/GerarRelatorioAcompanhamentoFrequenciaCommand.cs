using MediatR;
using SME.SR.Infra;
using System;

namespace SME.SR.Application
{
    public class GerarRelatorioAcompanhamentoFrequenciaCommand : IRequest
    {

        public GerarRelatorioAcompanhamentoFrequenciaCommand(RelatorioFrequenciaIndividualDto relatorio, Guid codigoCorrelacao)
        {
            Relatorio = relatorio;
            CodigoCorrelacao = codigoCorrelacao;
        }

        public RelatorioFrequenciaIndividualDto Relatorio { get; }
        public Guid CodigoCorrelacao { get; }
    }
}
