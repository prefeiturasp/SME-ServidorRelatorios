using MediatR;
using SME.SR.Infra;

namespace SME.SR.Application.Queries.RelatorioFaltasFrequencia
{
    public class ObterRelatorioFaltasFrequenciaPdfQuery : IRequest<RelatorioFaltasFrequenciaDto>
    {
        public ObterRelatorioFaltasFrequenciaPdfQuery(ObterRelatorioFaltasFrequenciasQuery filtro)
        {
            Filtro = filtro;
        }

        public ObterRelatorioFaltasFrequenciasQuery Filtro { get; set; }
    }
}
