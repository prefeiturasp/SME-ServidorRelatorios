using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterNotasFinaisPorTurmaQueryHandler : IRequestHandler<ObterNotasFinaisPorTurmaQuery, IEnumerable<NotaConceitoBimestreComponente>>
    {
        private IConselhoClasseNotaRepository conselhoClasseNotaRepository;

        public ObterNotasFinaisPorTurmaQueryHandler(IConselhoClasseNotaRepository conselhoClasseNotaRepository)
        {
            this.conselhoClasseNotaRepository = conselhoClasseNotaRepository ?? throw new ArgumentNullException(nameof(conselhoClasseNotaRepository));
        }

        public async Task<IEnumerable<NotaConceitoBimestreComponente>> Handle(ObterNotasFinaisPorTurmaQuery request, CancellationToken cancellationToken)
        {
            return await conselhoClasseNotaRepository.ObterNotasFinaisPorTurma(request.TurmaCodigo);
        }
    }
}
