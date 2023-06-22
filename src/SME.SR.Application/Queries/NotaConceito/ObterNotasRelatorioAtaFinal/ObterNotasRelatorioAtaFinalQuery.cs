using MediatR;
using SME.SR.Data;
using System.Collections.Generic;
using System.Linq;

namespace SME.SR.Application
{
    public class ObterNotasRelatorioAtaFinalQuery : IRequest<IEnumerable<IGrouping<string, NotasAlunoBimestre>>>
    {
        public ObterNotasRelatorioAtaFinalQuery(string[] codigosAlunos, string[] codigosTurmas, int anoLetivo, int modalidade, int semestre, int[] tiposTurma)
        {
            CodigosAlunos = codigosAlunos;
            CodigosTurmas = codigosTurmas;
            AnoLetivo = anoLetivo;
            Modalidade = modalidade;
            Semestre = semestre;
            TiposTurma = tiposTurma;
        }

        public string[] CodigosAlunos { get; set; }
        public string[] CodigosTurmas { get; set; }
        public int AnoLetivo { get; }
        public int Modalidade { get; }
        public int Semestre { get; }
        public int[] TiposTurma { get; }
    }
}