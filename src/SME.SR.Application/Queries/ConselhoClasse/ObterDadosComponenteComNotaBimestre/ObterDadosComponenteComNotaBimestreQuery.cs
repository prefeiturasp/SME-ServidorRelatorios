using MediatR;
using SME.SR.Data;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterDadosComponenteComNotaBimestreQuery : IRequest<IEnumerable<GrupoMatrizComponenteComNotaBimestre>>
    {
        public long FechamentoTurmaId { get; set; }
        public long ConselhoClasseId { get; set; }
        public Turma Turma { get; set; }
        public string CodigoAluno { get; set; }
        public PeriodoEscolar PeriodoEscolar { get; set; }
    }
}

