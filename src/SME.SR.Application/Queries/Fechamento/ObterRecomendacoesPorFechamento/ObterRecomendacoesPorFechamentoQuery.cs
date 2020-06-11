using MediatR;
using SME.SR.Data;

namespace SME.SR.Application
{
    public class ObterRecomendacoesPorFechamentoQuery :IRequest<RecomendacaoConselhoClasseAluno>
    {
        public long FechamentoTurmaId { get; set; }
        public string CodigoAluno { get; set; }

    }
}
