using MediatR;
using SME.SR.Data;
using System.Collections.Generic;
using System.Linq;

namespace SME.SR.Application
{
    public class ObterNotasFrequenciaRelatorioBoletimQuery : IRequest<IEnumerable<IGrouping<string, NotasFrequenciaAlunoBimestre>>>
    {
        public string[] CodigosTurma { get; set; }
        public string[] CodigosAlunos { get; set; }
    }
}
