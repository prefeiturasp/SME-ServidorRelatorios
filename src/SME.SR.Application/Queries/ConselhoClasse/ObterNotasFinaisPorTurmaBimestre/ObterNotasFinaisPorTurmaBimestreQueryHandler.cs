using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterNotasFinaisPorTurmaBimestreQueryHandler : IRequestHandler<ObterNotasFinaisPorTurmaBimestreQuery, IEnumerable<NotaConceitoBimestreComponente>>
    {
        private IConselhoClasseNotaRepository conselhoClasseNotaRepository;

        public ObterNotasFinaisPorTurmaBimestreQueryHandler(IConselhoClasseNotaRepository conselhoClasseNotaRepository)
        {
            this.conselhoClasseNotaRepository = conselhoClasseNotaRepository ?? throw new ArgumentNullException(nameof(conselhoClasseNotaRepository));
        }

        public async Task<IEnumerable<NotaConceitoBimestreComponente>> Handle(ObterNotasFinaisPorTurmaBimestreQuery request, CancellationToken cancellationToken)
        {
            return await conselhoClasseNotaRepository.ObterNotasFinaisPorTurmaBimestre(request.TurmaCodigo, request.Bimestres);
        }
    }
}
