using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterFechamentoConsolidadoPorTurmasBimestreQuery : IRequest<IEnumerable<FechamentoConsolidadoComponenteTurmaDto>>
    {
        public ObterFechamentoConsolidadoPorTurmasBimestreQuery(string[] turmasCodigo, int[] bimestres, int? situacaoFechamento)
        {
            TurmasCodigo = turmasCodigo;
            Bimestres = bimestres;
            SituacaoFechamento = situacaoFechamento;
        }

        public string[] TurmasCodigo { get; internal set; }
        public int[] Bimestres { get; internal set; }
        public int? SituacaoFechamento { get; internal set; }
    }
}
