using MediatR;
using SME.SR.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterPareceresConclusivosPorTipoAprovacaoQueryHandler : IRequestHandler<ObterPareceresConclusivosPorTipoAprovacaoQuery, IEnumerable<long>>
    {
        private readonly IConselhoClasseRepository conselhoClasseRepository;

        public ObterPareceresConclusivosPorTipoAprovacaoQueryHandler(IConselhoClasseRepository conselhoClasseRepository)
        {
            this.conselhoClasseRepository = conselhoClasseRepository ?? throw new ArgumentNullException(nameof(conselhoClasseRepository));
        }
        public async Task<IEnumerable<long>> Handle(ObterPareceresConclusivosPorTipoAprovacaoQuery request, CancellationToken cancellationToken)
        {
            return await conselhoClasseRepository.ObterPareceresConclusivosPorTipoAprovacao(request.Aprovado);
            
        }
    }
}
