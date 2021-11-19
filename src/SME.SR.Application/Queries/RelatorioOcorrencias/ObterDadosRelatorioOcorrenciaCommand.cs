using MediatR;
using SME.SR.Infra;

namespace SME.SR.Application
{
    public class ObterDadosRelatorioOcorrenciaQuery : IRequest<RelatorioRegistroOcorrenciasDto>
    {
        public FiltroImpressaoOcorrenciaDto FiltroOcorrencia { get; set; }

        public ObterDadosRelatorioOcorrenciaQuery(FiltroImpressaoOcorrenciaDto filtroOcorrencia)
        {
            FiltroOcorrencia = filtroOcorrencia;
        }
    }
}