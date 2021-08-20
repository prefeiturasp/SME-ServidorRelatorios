using MediatR;
using SME.SR.Data;
using System;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterDadosAlunosPorCodigosQuery : IRequest<IEnumerable<AlunoHistoricoEscolar>>
    {
        public ObterDadosAlunosPorCodigosQuery(long[] codigosAluno, int? anoLetivo = null)
        {
            CodigosAluno = codigosAluno;
            AnoLetivo = anoLetivo;
        }

        public long[] CodigosAluno { get; set; }
        public int? AnoLetivo { get; set; }
    }
}
