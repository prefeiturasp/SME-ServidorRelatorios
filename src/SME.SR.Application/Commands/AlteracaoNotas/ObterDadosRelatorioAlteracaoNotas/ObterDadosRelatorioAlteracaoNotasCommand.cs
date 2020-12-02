using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterDadosRelatorioAlteracaoNotasCommand : IRequest<List<TurmaAlteracaoNotasDto>>
    {
        public ObterDadosRelatorioAlteracaoNotasCommand(FiltroRelatorioAlteracaoNotasDto filtroRelatorio)
        {
            FiltroRelatorio = filtroRelatorio;
        }

        public FiltroRelatorioAlteracaoNotasDto FiltroRelatorio { get; set; }
    }
}
