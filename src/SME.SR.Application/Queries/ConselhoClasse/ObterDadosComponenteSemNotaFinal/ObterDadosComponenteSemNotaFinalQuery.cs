using MediatR;
using SME.SR.Data;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterDadosComponenteSemNotaFinalQuery : IRequest<IEnumerable<GrupoMatrizComponenteSemNotaFinal>>
    {
        public string CodigoTurma { get; set; }
        public string CodigoAluno { get; set; }
        public int? Bimestre { get; set; }
    }
}
