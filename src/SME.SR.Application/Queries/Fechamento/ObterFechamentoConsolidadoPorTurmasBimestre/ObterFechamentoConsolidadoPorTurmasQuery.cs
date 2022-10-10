using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterFechamentoConsolidadoPorTurmasQuery : IRequest<IEnumerable<FechamentoConsolidadoComponenteTurmaDto>>
    {
        public ObterFechamentoConsolidadoPorTurmasQuery(string[] turmasCodigo,int[]semestres, int[] bimestres)
        {
            TurmasCodigo = turmasCodigo;
            Semestres = semestres;
            Bimestres = bimestres;
        }
        public string[] TurmasCodigo { get; internal set; }
        public int[] Semestres { get; internal set; }
        public int[] Bimestres { get; internal set; }
    }
}
