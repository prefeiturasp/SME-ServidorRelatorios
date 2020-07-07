using MediatR;
using SME.SR.Infra;

namespace SME.SR.Application
{
    public class ObterRelatorioFaltasFrequenciasQuery : IRequest<RelatorioFaltasFrequenciasDto>
    {
        public TipoFormatoRelatorio TipoFormatoRelatorio { get; set; }
    }
}
