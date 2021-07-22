using MediatR;
using SME.SR.Infra;

namespace SME.SR.Application.Queries.RelatorioFaltasFrequencia
{
    public class ObterRelatorioFrequenciaPdfQuery : IRequest<RelatorioFaltasFrequenciaDto>
    {
        public ObterRelatorioFrequenciaPdfQuery(FiltroRelatorioFrequenciaDto filtro)
        {
            Filtro = filtro;
        }

        public FiltroRelatorioFrequenciaDto Filtro { get; set; }
    }
}
