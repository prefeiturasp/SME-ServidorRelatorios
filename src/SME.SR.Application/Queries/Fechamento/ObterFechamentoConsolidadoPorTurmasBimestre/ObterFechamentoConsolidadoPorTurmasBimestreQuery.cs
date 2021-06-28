using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterFechamentoConsolidadoPorTurmasBimestreQuery : IRequest<IEnumerable<FechamentoConsolidadoComponenteTurmaDto>>
    {
        public ObterFechamentoConsolidadoPorTurmasBimestreQuery(long[] turmasId, int bimestre, int situacaoFechamento)
        {
            TurmasId = turmasId;
            Bimestre = bimestre;
            SituacaoFechamento = situacaoFechamento;
        }

        public long[] TurmasId { get; internal set; }
        public int Bimestre { get; internal set; }
        public int SituacaoFechamento { get; internal set; }
    }
}
