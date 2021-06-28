using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterConselhosClasseConsolidadoPorTurmasBimestreQueryHandler : IRequestHandler<ObterConselhosClasseConsolidadoPorTurmasBimestreQuery, IEnumerable<ConselhoClasseConsolidadoTurmaAlunoDto>>
    {
        private readonly IConselhoClasseConsolidadoRepository conselhoClasseConsolidadoRepository;

        public ObterConselhosClasseConsolidadoPorTurmasBimestreQueryHandler(IConselhoClasseConsolidadoRepository conselhoClasseConsolidadoRepository)
        {
            this.conselhoClasseConsolidadoRepository = conselhoClasseConsolidadoRepository;
        }

        public async Task<IEnumerable<ConselhoClasseConsolidadoTurmaAlunoDto>> Handle(ObterConselhosClasseConsolidadoPorTurmasBimestreQuery request, CancellationToken cancellationToken)
        {
            return await conselhoClasseConsolidadoRepository.ObterConselhosClasseConsolidadoPorTurmasBimestreAsync(request.TurmasId,
                                                                                                     request.Bimestre,
                                                                                                     request.SituacaoFechamento);
        }
    }
}
