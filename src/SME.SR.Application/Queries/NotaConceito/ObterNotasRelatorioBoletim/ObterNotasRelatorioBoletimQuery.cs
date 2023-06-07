using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System.Collections.Generic;
using System.Linq;

namespace SME.SR.Application
{
    public class ObterNotasRelatorioBoletimQuery : IRequest<IEnumerable<IGrouping<string, NotasAlunoBimestre>>>
    {
        public ObterNotasRelatorioBoletimQuery(string[] codigosAlunos, string[] codigosTurmas, int anoLetivo, int modalidade, int semestre, IDictionary<string, string> tipoNotas)
        {
            CodigosAlunos = codigosAlunos;
            CodigosTurmas = codigosTurmas;
            AnoLetivo = anoLetivo;
            Modalidade = modalidade;
            Semestre = semestre;
            TipoNotas = tipoNotas;
        }

        public string[] CodigosAlunos { get; set; }
        public string[] CodigosTurmas { get; set; }
        public int AnoLetivo { get; }
        public int Modalidade { get; }
        public int Semestre { get; }
        public IDictionary<string, string> TipoNotas {  get; set; } 
    }
}
