using MediatR;
using SME.SR.Infra;

namespace SME.SR.Application
{
    public class ObterDadosRelatorioAcompanhamentoFrequenciaCommand : IRequest<RelatorioFrequenciaIndividualDto>
    {
        public FiltroAcompanhamentoFrequenciaJustificativaDto FiltroRelatorio { get; set; }

        public ObterDadosRelatorioAcompanhamentoFrequenciaCommand(FiltroAcompanhamentoFrequenciaJustificativaDto filtroRelatorio)
        {
            FiltroRelatorio = filtroRelatorio;
        }
    }
}
