using MediatR;
using SME.SR.Infra;

namespace SME.SR.Application.Queries.RelatorioFaltasFrequencia
{
    public class ObterRelatorioFrequenciaPdfQuery : IRequest<RelatorioFrequenciaDto>
    {
        public ObterRelatorioFrequenciaPdfQuery(FiltroRelatorioFrequenciasDto filtro)
        {
            Filtro = filtro;
        }

        public FiltroRelatorioFrequenciasDto Filtro { get; set; }
    }
}
