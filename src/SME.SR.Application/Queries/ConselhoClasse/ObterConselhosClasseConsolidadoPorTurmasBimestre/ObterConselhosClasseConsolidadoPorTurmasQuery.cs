using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterConselhosClasseConsolidadoPorTurmasQuery : IRequest<IEnumerable<ConselhoClasseConsolidadoTurmaAlunoDto>>
    {
        public ObterConselhosClasseConsolidadoPorTurmasQuery(string[] turmasCodigo,bool todasUe)
        {
            TurmasCodigo = turmasCodigo;
            TodasUe = todasUe;
        }
        public bool TodasUe { get; set; }
        public string[] TurmasCodigo { get; internal set; }
    }
}
