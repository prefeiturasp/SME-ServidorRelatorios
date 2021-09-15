using MediatR;
using SME.SR.Data;
using System.Collections.Generic;
using System.Linq;

namespace SME.SR.Application
{
    public class ObterNotasRelatorioAtaBimestralQuery : IRequest<IEnumerable<IGrouping<string, NotasAlunoBimestre>>>
    {
        public ObterNotasRelatorioAtaBimestralQuery(string[] codigosAlunos, int anoLetivo, int modalidade, int semestre, int[] tiposTurma, int bimestre)
        {
            CodigosAlunos = codigosAlunos;
            AnoLetivo = anoLetivo;
            Modalidade = modalidade;
            Semestre = semestre;
            TiposTurma = tiposTurma;
            Bimestre = bimestre;
        }

        public string[] CodigosAlunos { get; set; }
        public int AnoLetivo { get; }
        public int Modalidade { get; }
        public int Semestre { get; }
        public int[] TiposTurma { get; }
        public int Bimestre { get; set; }
    }
}