using MediatR;
using SME.SR.Workers.SGP.Models;
using System.Collections.Generic;

namespace SME.SR.Workers.SGP.Queries
{
    public class ObterDadosComponenteComNotaFinalQuery : IRequest<IEnumerable<GrupoMatrizComponenteComNotaFinal>>
    {
        public long FechamentoTurmaId { get; set; }
        public long ConselhoClasseId { get; set; }
        public Turma Turma { get; set; }
        public string CodigoAluno { get; set; }
        public PeriodoEscolar PeriodoEscolar { get; set; }
    }
}
