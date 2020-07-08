using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterNotasEFrequenciasDosAlunosQuery : IRequest<IEnumerable<HistoricoEscolarDTO>>
    {
        public string CodigoTurma { get; set; }
        public string[] CodigoAlunos { get; set; }
    }
}
