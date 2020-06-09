using MediatR;
using SME.SR.Workers.SGP.Commons;
using SME.SR.Workers.SGP.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Workers.SGP.Queries
{
    public class ObterNotasAlunoBimestreQueryHandler : IRequestHandler<ObterNotasAlunoBimestreQuery, IEnumerable<NotaConceitoBimestreComponente>>
    {
        private IFechamentoNotaRepository _fechamentoNotaRepository;
        private IConselhoClasseNotaRepository _conselhoClasseNotaRepository;

        public ObterNotasAlunoBimestreQueryHandler(IFechamentoNotaRepository fechamentoNotaRepository,
                                                   IConselhoClasseNotaRepository conselhoClasseNotaRepository)
        {
            this._fechamentoNotaRepository = fechamentoNotaRepository;
            this._conselhoClasseNotaRepository = conselhoClasseNotaRepository;
        }
      
        public async Task<IEnumerable<NotaConceitoBimestreComponente>> Handle(ObterNotasAlunoBimestreQuery request, CancellationToken cancellationToken)
        {
            if (request.Bimestre.HasValue)
            {
                return await _fechamentoNotaRepository.ObterNotasAlunoBimestre(request.FechamentoTurmaId, request.CodigoAluno);
            }
            else
            {
                return await _conselhoClasseNotaRepository.ObterNotasFinaisAlunoBimestre(request.CodigoTurma, request.CodigoAluno);
            }
        }
    }
}
