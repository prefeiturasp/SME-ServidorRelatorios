using MediatR;
using SME.SR.Workers.SGP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SME.SR.Workers.SGP.Queries
{
    public class ObterDadosComponenteSemNotaBimestreQuery : IRequest<IEnumerable<GrupoMatrizComponenteSemNotaBimestre>>
    {
        public string CodigoTurma { get; set; }
        public string CodigoAluno { get; set; }
        public int? Bimestre { get; set; }
    }
}
