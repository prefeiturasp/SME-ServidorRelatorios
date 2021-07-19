using MediatR;
using SME.SR.Infra;

namespace SME.SR.Application.Queries.RelatorioFaltasFrequencia
{
    public class ObterRelatorioFaltasFrequenciaPdfQuery : IRequest<RelatorioFaltasFrequenciaDto>
    {
        public ObterRelatorioFaltasFrequenciaPdfQuery(FiltroRelatorioFrequenciasDto filtro)
        {
            Filtro = filtro;
        }

        public FiltroRelatorioFrequenciasDto Filtro { get; set; }
    }
}
