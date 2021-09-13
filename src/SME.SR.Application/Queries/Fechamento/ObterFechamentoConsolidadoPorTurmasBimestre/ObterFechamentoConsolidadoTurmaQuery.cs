using MediatR;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Application
{
    public class ObterFechamentoConsolidadoTurmaQuery : IRequest<IEnumerable<FechamentoConsolidadoTurmaDto>>
    {
        public string[] TurmasCodigo { get; internal set; }

        public ObterFechamentoConsolidadoTurmaQuery(string[] turmasCodigo)
        {
            TurmasCodigo = turmasCodigo;
        }
    }
}
