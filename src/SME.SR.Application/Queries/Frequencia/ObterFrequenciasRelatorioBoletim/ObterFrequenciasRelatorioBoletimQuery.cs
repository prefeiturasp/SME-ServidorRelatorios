using MediatR;
using SME.SR.Data;
using System.Collections.Generic;
using System.Linq;

namespace SME.SR.Application
{
   public class ObterFrequenciasRelatorioBoletimQuery : IRequest<IEnumerable<IGrouping<string, FrequenciaAluno>>>
    {
        public string[] CodigosTurma { get; set; }
        public string[] CodigosAluno { get; set; }

    }
}
