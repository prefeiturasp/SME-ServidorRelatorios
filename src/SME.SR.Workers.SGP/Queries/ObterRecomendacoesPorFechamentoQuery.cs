using MediatR;
using SME.SR.Workers.SGP.Models;

namespace SME.SR.Workers.SGP.Queries
{
    public class ObterRecomendacoesPorFechamentoQuery :IRequest<RecomendacaoConselhoClasseAluno>
    {
        public long FechamentoTurmaId { get; set; }
        public string CodigoAluno { get; set; }

    }
}
