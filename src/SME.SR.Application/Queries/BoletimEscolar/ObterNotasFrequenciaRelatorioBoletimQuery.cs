using MediatR;
using SME.SR.Data;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterNotasFrequenciaRelatorioBoletimQuery : IRequest<IEnumerable<NotasFrequenciaAlunoBimestre>>
    {
        public string[] CodigosTurma { get; set; }
        public string[] CodigosAlunos { get; set; }
    }
}
