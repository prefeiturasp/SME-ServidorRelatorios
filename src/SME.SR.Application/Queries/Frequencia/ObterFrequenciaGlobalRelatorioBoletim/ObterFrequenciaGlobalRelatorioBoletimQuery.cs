using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System.Collections.Generic;
using System.Linq;

namespace SME.SR.Application
{
   public class ObterFrequenciaGlobalRelatorioBoletimQuery : IRequest<IEnumerable<IGrouping<string, FrequenciaAluno>>>
    {
        public ObterFrequenciaGlobalRelatorioBoletimQuery(string[] codigosAluno, int anoLetivo, Modalidade modalidade)
        {
            CodigosAluno = codigosAluno;
            AnoLetivo = anoLetivo;
            Modalidade = modalidade;
        }

        public string[] CodigosAluno { get; set; }
        public int AnoLetivo { get; }
        public Modalidade Modalidade { get; }
    }
}
