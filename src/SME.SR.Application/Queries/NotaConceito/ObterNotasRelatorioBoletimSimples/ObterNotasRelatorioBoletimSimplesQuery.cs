using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System.Collections.Generic;
using System.Linq;

namespace SME.SR.Application
{
    public class ObterNotasRelatorioBoletimSimplesQuery : IRequest<IEnumerable<IGrouping<string, NotasAlunoBimestreBoletimSimplesDto>>>
    {
        public ObterNotasRelatorioBoletimSimplesQuery(string[] codigosAlunos, string[] codigosTurmas, int anoLetivo, int modalidade, int semestre)
        {
            CodigosAlunos = codigosAlunos;
            CodigosTurmas = codigosTurmas;
            AnoLetivo = anoLetivo;
            Modalidade = modalidade;
            Semestre = semestre;
        }

        public string[] CodigosAlunos { get; set; }
        public string[] CodigosTurmas { get; set; }
        public int AnoLetivo { get; }
        public int Modalidade { get; }
        public int Semestre { get; }
    }
}
