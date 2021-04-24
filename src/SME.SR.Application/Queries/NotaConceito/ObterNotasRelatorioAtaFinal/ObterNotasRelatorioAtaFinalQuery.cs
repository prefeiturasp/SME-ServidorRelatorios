using MediatR;
using SME.SR.Data;
using System.Collections.Generic;
using System.Linq;

namespace SME.SR.Application
{
    public class ObterNotasRelatorioAtaFinalQuery : IRequest<IEnumerable<IGrouping<string, NotasAlunoBimestre>>>
    {
        public ObterNotasRelatorioAtaFinalQuery(string[] codigosAlunos, int anoLetivo, int modalidade, int semestre, int tipoTurma)
        {
            CodigosAlunos = codigosAlunos;
            AnoLetivo = anoLetivo;
            Modalidade = modalidade;
            Semestre = semestre;
            TipoTurma = tipoTurma;
        }

        public string[] CodigosAlunos { get; set; }
        public int AnoLetivo { get; }
        public int Modalidade { get; }
        public int Semestre { get; }
        public int TipoTurma { get; }
    }
}