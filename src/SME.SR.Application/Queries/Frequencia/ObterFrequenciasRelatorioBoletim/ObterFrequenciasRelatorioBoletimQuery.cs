using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System.Collections.Generic;
using System.Linq;

namespace SME.SR.Application
{
   public class ObterFrequenciasRelatorioBoletimQuery : IRequest<IEnumerable<IGrouping<string, FrequenciaAluno>>>
    {
        public ObterFrequenciasRelatorioBoletimQuery(string[] codigosAluno, int anoLetivo, Modalidade modalidade, int semestre, string turmaCodigo)
        {
            CodigosAluno = codigosAluno;
            AnoLetivo = anoLetivo;
            Modalidade = modalidade;
            Semestre = semestre;
            TurmaCodigo = turmaCodigo;
        }

        public string[] CodigosAluno { get; set; }
        public int AnoLetivo { get; }
        public Modalidade Modalidade { get; }
        public int Semestre { get; }
        public string TurmaCodigo { get; set; }
    }
}
