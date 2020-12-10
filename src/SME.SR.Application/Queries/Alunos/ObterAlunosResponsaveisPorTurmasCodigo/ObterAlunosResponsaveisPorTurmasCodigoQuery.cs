using MediatR;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Application
{
    public class ObterAlunosResponsaveisPorTurmasCodigoQuery : IRequest<IEnumerable<AlunoResponsavelAdesaoAEDto>>
    {
        public ObterAlunosResponsaveisPorTurmasCodigoQuery(long[] turmasCodigo)
        {
            TurmasCodigo = turmasCodigo;
        }

        public long[] TurmasCodigo{ get; set; }
    }
}
