using MediatR;
using SME.SR.Infra;

namespace SME.SR.Application
{
    public class ObterDadosRelatorioOcorrenciaCommand : IRequest<RelatorioRegistroOcorrenciasDto>
    {
        public FiltroImpressaoOcorrenciaDto FiltroOcorrencia { get; set; }

        public ObterDadosRelatorioOcorrenciaCommand(FiltroImpressaoOcorrenciaDto filtroOcorrencia)
        {
            FiltroOcorrencia = filtroOcorrencia;
        }
    }
}