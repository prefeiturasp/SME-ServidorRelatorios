using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterDadosRelatorioUsuariosCommand : IRequest<DadosRelatorioUsuariosDto>
    {
        public ObterDadosRelatorioUsuariosCommand(FiltroRelatorioUsuariosDto filtroRelatorio)
        {
            FiltroRelatorio = filtroRelatorio;
        }

        public FiltroRelatorioUsuariosDto FiltroRelatorio { get; set; }
    }
}
