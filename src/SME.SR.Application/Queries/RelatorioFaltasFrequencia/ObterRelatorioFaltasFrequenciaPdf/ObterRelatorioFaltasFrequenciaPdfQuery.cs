using MediatR;
using SME.SR.Infra;

namespace SME.SR.Application.Queries.RelatorioFaltasFrequencia
{
    public class ObterRelatorioFaltasFrequenciaPdfQuery : IRequest<RelatorioFaltasFrequenciaDto>
    {
        public ObterRelatorioFaltasFrequenciaPdfQuery(FiltroRelatorioFaltasFrequenciasDto filtro)
        {
            Filtro = filtro;
        }

        public FiltroRelatorioFaltasFrequenciasDto Filtro { get; set; }
    }
}
