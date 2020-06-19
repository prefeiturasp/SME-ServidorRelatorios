using MediatR;
using SME.SR.Data;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterDadosComponenteComNotaFinalQuery : IRequest<IEnumerable<GrupoMatrizComponenteComNotaFinal>>
    {
        public long FechamentoTurmaId { get; set; }
        public long ConselhoClasseId { get; set; }
        public Turma Turma { get; set; }
        public string CodigoAluno { get; set; }
        public PeriodoEscolar PeriodoEscolar { get; set; }
        public Usuario Usuario { get; set; }
    }
}
