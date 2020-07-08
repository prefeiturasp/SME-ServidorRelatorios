using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterNotasAlunoBimestreQueryHandler : IRequestHandler<ObterNotasAlunoBimestreQuery, IEnumerable<NotaConceitoBimestreComponente>>
    {
        private IFechamentoNotaRepository fechamentoNotaRepository;
        private IConselhoClasseNotaRepository conselhoClasseNotaRepository;

        public ObterNotasAlunoBimestreQueryHandler(IFechamentoNotaRepository fechamentoNotaRepository,
                                                   IConselhoClasseNotaRepository conselhoClasseNotaRepository)
        {
            this.fechamentoNotaRepository = fechamentoNotaRepository ?? throw new ArgumentNullException(nameof(fechamentoNotaRepository));
            this.conselhoClasseNotaRepository = conselhoClasseNotaRepository ?? throw new ArgumentNullException(nameof(conselhoClasseNotaRepository));
        }
      
        public async Task<IEnumerable<NotaConceitoBimestreComponente>> Handle(ObterNotasAlunoBimestreQuery request, CancellationToken cancellationToken)
        {
            if (request.Bimestre.HasValue)
            {
                return await fechamentoNotaRepository.ObterNotasAlunoBimestre(request.FechamentoTurmaId, request.CodigoAluno);
            }
            else
            {
                return await conselhoClasseNotaRepository.ObterNotasFinaisAlunoBimestre(request.CodigoTurma, request.CodigoAluno);
            }
        }
    }
}
