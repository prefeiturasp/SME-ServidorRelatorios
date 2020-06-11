using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
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
