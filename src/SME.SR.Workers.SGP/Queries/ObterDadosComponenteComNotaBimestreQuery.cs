using MediatR;
using SME.SR.Workers.SGP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SME.SR.Workers.SGP.Queries
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

