using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterConselhosClasseConsolidadoPorTurmasQueryHandler : IRequestHandler<ObterConselhosClasseConsolidadoPorTurmasQuery, IEnumerable<ConselhoClasseConsolidadoTurmaAlunoDto>>
    {
        private readonly IConselhoClasseConsolidadoRepository conselhoClasseConsolidadoRepository;

        public ObterConselhosClasseConsolidadoPorTurmasQueryHandler(IConselhoClasseConsolidadoRepository conselhoClasseConsolidadoRepository)
        {
            this.conselhoClasseConsolidadoRepository = conselhoClasseConsolidadoRepository;
        }

        public async Task<IEnumerable<ConselhoClasseConsolidadoTurmaAlunoDto>> Handle(ObterConselhosClasseConsolidadoPorTurmasQuery request, CancellationToken cancellationToken)
        {
            return await conselhoClasseConsolidadoRepository.ObterConselhosClasseConsolidadoPorTurmasAsync(request.TurmasCodigo);
        }
    }
}
