using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterConselhosClasseConsolidadoPorTurmasQuery : IRequest<IEnumerable<ConselhoClasseConsolidadoTurmaAlunoDto>>
    {
        public ObterConselhosClasseConsolidadoPorTurmasQuery(string[] turmasCodigo)
        {
            TurmasCodigo = turmasCodigo;
        }

        public string[] TurmasCodigo { get; internal set; }
    }
}
